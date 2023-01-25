using System;
using System.Threading;
class State
{
    public char replace;
    public Shift shift;
    public int newState;
    public State(char replace, Shift shift, int newState)
    {
        this.replace = replace; 
        this.shift = shift;   
        this.newState = newState;   
    }

}
internal enum Shift
{
    Left,
    Right
}
class Row
{
    public char symbol;
    public State[] state;
    public Row(char symbol, State[] state)
    {
        this.symbol = symbol;
        this.state = state; 
    }
}
class TransMatrix
{
    public Row[] rows;
    public TransMatrix(Row[]rows)
    {
        this.rows = rows;
    }
    public State GetRow(char symbol, int state)
    {
        foreach(var row in rows )
        {
            if(row.symbol == symbol)
            {
                return row.state[state];
            }
        }
        throw new NotImplementedException();
    }
}
class TuringMachine
{
    public int sHead = 0;
    public int Head { get; set; }
    public int State { get; set; }
    public char[] tapes = new char[5];
    public char[] headPos = new char[5];
    public readonly TransMatrix TransMatrix;
    public TuringMachine(TransMatrix transMatrix)
    {
        TransMatrix = transMatrix;
        Array.Fill(tapes, '_');
        Array.Fill(headPos, '_');
    }
    public void Inputs(int index, string str)
    {
        Pointing1:
        for(int i = 0; i<str.Length; i++)
        {
            if(i + index + tapes.Length / 4 < str.Length)
            {
                Size();
                goto Pointing1;
            }
            tapes[i + index + tapes.Length / 4] = str[i];
        }
        headPos = new char[tapes.Length];
        Array.Fill(headPos, '_');
        headPos[tapes.Length / 4] = '|';
    }
    public string ShowStr(int index1, int length)
    {
        return new string(tapes, index1+tapes.Length/4, length);   
    }
    public string ShowHead(int index, int length, int temp)
    {
        if(temp == 0)
        {
            return new string(headPos, index+tapes.Length/4, length); 
        }
        else if(temp >= 1)
        {
            headPos[temp - 1 + index + tapes.Length / 4] = '_';
            headPos[temp + index + tapes.Length / 4] = '|';
            return new string(headPos, index+tapes.Length/4, length);
        }
        return new string(headPos, index+tapes.Length/4, length);   
    }
    public void Step()
    {
        State state = TransMatrix.GetRow(tapes[Head], State);
        tapes[Head] = state.replace;
        if(state.shift == Shift.Left)
        {
            Head--;
            if(Head < 0)
            {
                Size();
            }
        }
        else if(state.shift == Shift.Right)
        {
            Head++;
            if(Head >= tapes.Length)
            {
                Size();
            }       
        }
        else
        {
            throw new NotImplementedException();
        }
        State = state.newState;
    }
    public void SetHead(int head)
    {
        Head = head + tapes.Length / 4;
    }
    public void Size()
    {
        char[] tm = tapes;
        tapes = new char[tm.Length * 2];
        Array.Fill(tapes, '_');
        for(int i = tapes.Length/4; i < tm.Length+tapes.Length/4; i++)
        {
            tapes[i] = tm[i-tapes.Length/4];
        }
        SetHead(sHead);
        headPos = new char[tapes.Length];
        Array.Fill(tapes, '_');
        headPos[tapes.Length/4] = '|';
    }
    public string CorrectnessCheck(string str, char[] abc)
    {
        for(int i = 0; i < str.Length; i++)
        {
            for(int j = 0; j < abc.Length; j++)
            {
                if(str[i] == abc[j])
                {
                    break;
                }
                //якщо дані введе користувач
                else if(j == abc.Length - 1 && str[i] != abc[j])
                {
                    Console.WriteLine("Incorrect data");
                    Console.WriteLine("Default word: 1001001110110");
                    str = "1001001110110";
                    return str;  
                }
            }
        }
        return str;
    }
}
class Program
{
    static void Main(string[] args)
    {
        char[] alphabet = new char[] { '0', '1', '_' };
        var turingMachine = new TuringMachine
            (
              new TransMatrix
              (
               new Row[]
               {
                   new Row('0', new State[]{new State('1', Shift.Right, 0)}),
                   new Row('1', new State[]{new State('0', Shift.Right, 0)}),
                   new Row('_', new State[]{new State('_', Shift.Right, -1)})
                }
               )
              );

        turingMachine.SetHead(turingMachine.sHead);
        turingMachine.State = 0;
        string str = "1001001110110";
        str = turingMachine.CorrectnessCheck(str, alphabet);
        turingMachine.Inputs(0, str);
        int time = 2500;
        int temp = 0;
        Console.WriteLine(turingMachine.ShowHead(0, str.Length+1, temp));
        Console.WriteLine(turingMachine.ShowStr(0, str.Length+1));
        while(turingMachine.State != 1)
        {
            Thread.Sleep(time);
            Console.Clear();
            turingMachine.Step();
            Console.WriteLine(turingMachine.ShowHead(0, str.Length+1, temp));
            Console.WriteLine(turingMachine.ShowStr(0, str.Length+1));
            Console.WriteLine("");
            Thread.Sleep(time);
            Console.Clear();
            Console.WriteLine(turingMachine.ShowHead(0, str.Length+1, temp));
            Console.WriteLine(turingMachine.ShowStr(0, str.Length+1));
            temp++;
                
        }
    }
}