using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PrvaDomacaZadaca_Kalkulator
{
    public class Factory
    {
        public static ICalculator CreateCalculator()
        {
            // vratiti kalkulator
            Kalkulator calc = new Kalkulator();
            calc.InitDisplay();
            calc.DefineClearingOperations();
            calc.DefineSpecificOperations();
            calc.DefineAllowedOpertions();
            return calc;
        }
    }

    public class Kalkulator:ICalculator
    {

        private int maxDecimals = 1000000000;
        private char endOperation = '=';
        private char commaOperation = ',';

        private String display;
        private LinkedList<char> AllowedOperations = new LinkedList<char>();

        private String memory = null;
        private String exception = "-E-";

        private LinkedList<char> SpecificOperations = new LinkedList<char>();
        private LinkedList<char> ClearingOperations = new LinkedList<char>();

        private String operation = null;
        private String firstValue = null;
        private String secondValue = null;

        public void InitDisplay()
        {
            display = "0";
        }
        public void DefineClearingOperations()
        {
            ClearingOperations.AddLast('C');
            ClearingOperations.AddLast('O');
        }
        public void DefineSpecificOperations()
        {
            SpecificOperations.AddLast('Q');
            SpecificOperations.AddLast('S');
            SpecificOperations.AddLast('K');
            SpecificOperations.AddLast('T');
            SpecificOperations.AddLast('M');
            SpecificOperations.AddLast('Q');
            SpecificOperations.AddLast('R');
            SpecificOperations.AddLast('I');
            SpecificOperations.AddLast('P');
            SpecificOperations.AddLast('G');
        }
        public void DefineAllowedOpertions()
        {
            AllowedOperations.AddLast('+');
            AllowedOperations.AddLast('-');
            AllowedOperations.AddLast('*');
            AllowedOperations.AddLast('/');
            AllowedOperations.AddLast('M');
            AllowedOperations.AddLast('S');
            AllowedOperations.AddLast('K');
            AllowedOperations.AddLast('T');
            AllowedOperations.AddLast('Q');
            AllowedOperations.AddLast('R');
            AllowedOperations.AddLast('I');
            AllowedOperations.AddLast('P');
            AllowedOperations.AddLast('G');
        }

        public void Press(char inPressedDigit)
        {
            if (display.Equals(exception))
                return;

            else if (ClearingOperations.Contains(inPressedDigit))
            {
                if (inPressedDigit.Equals('C'))
                {
                    InitDisplay();

                    if (!(String.IsNullOrEmpty(secondValue)))
                        secondValue = null;
                    else
                        firstValue = null;
                }
                else
                {
                    operation = null;
                    firstValue = null;
                    secondValue = null;
                    InitDisplay();
                }
            }

            else if (endOperation.Equals(inPressedDigit))
            {
                if (!(String.IsNullOrEmpty(firstValue)) && !(String.IsNullOrEmpty(secondValue)) && !(String.IsNullOrEmpty(operation)))
                {
                    CalculateResult();
                    firstValue = null;
                    secondValue = null;
                    operation = null;
                    //This is when we calc the operations
                }
                else
                {
                    if (String.IsNullOrEmpty(firstValue))
                        InitDisplay();
                    else
                    {
                        if (!(String.IsNullOrEmpty(operation)))
                        {
                            secondValue = firstValue;
                            CalculateResult();
                        }
                        else
                            display = ReturnFormatedString(firstValue);
                    }
                }
            } //Needs to be finished (=)

            else if (String.IsNullOrEmpty(firstValue))
            {
                if (Char.IsDigit(inPressedDigit) || inPressedDigit.Equals('-'))
                    firstValue = Char.ToString(inPressedDigit);

                else if(inPressedDigit.Equals(commaOperation))
                    firstValue = "0" + Char.ToString(inPressedDigit);

                else
                {
                    //Console.WriteLine("First character needs to be a digit or a -");
                    display = exception;
                    return;
                }

                display = firstValue;
            }

            else
            {
                if (firstValue.Equals("-") && Char.IsDigit(inPressedDigit)) //When first case is negative value
                {
                    firstValue += Char.ToString(inPressedDigit);
                    display = firstValue;
                }

                else if (Char.IsDigit(inPressedDigit)) //When input char is a digit
                {
                    if (String.IsNullOrEmpty(operation) || firstValue.EndsWith(",")) //Element before was a digit or an operation
                    {
                        firstValue += Char.ToString(inPressedDigit);
                        display = firstValue;
                    }
                    else if (!(String.IsNullOrEmpty(operation)))
                    {
                        if (String.IsNullOrEmpty(secondValue))
                            secondValue = Char.ToString(inPressedDigit);
                        else
                            secondValue += Char.ToString(inPressedDigit);

                        display = secondValue;
                    }
                    else //A specific operation was before and a next element needs to be a digit or a end operation 
                    {
                        display = exception;
                        //Console.WriteLine("There was a specific operation before"); // ex. Q
                        return;
                    }
                }

                else if (AllowedOperations.Contains(inPressedDigit))
                {
                    display = ReturnFormatedString(display);
                    if (!(String.IsNullOrEmpty(operation)))
                    {
                        if (String.IsNullOrEmpty(secondValue))
                        {
                            if (SpecificOperations.Contains(inPressedDigit))
                            {
                                String tmpOperation = operation;
                                String tmpFirstValue = firstValue;
                                operation = Char.ToString(inPressedDigit);
                                CalculateResult();
                                operation = tmpOperation;
                                firstValue = tmpFirstValue;
                            }
                                
                            else
                                operation = Char.ToString(inPressedDigit); //This is the new operation
                        }
                        else
                        {
                            if (SpecificOperations.Contains(inPressedDigit))
                            {
                                double second = SpecificOperationCounter(inPressedDigit, double.Parse(secondValue));
                                secondValue = second.ToString("G9");
                                if ((int)second / maxDecimals < 10)
                                    display = second.ToString("G9");
                                else
                                    display = exception;
                                return;
                            }

                            CalculateResult();
                            operation = Char.ToString(inPressedDigit);

                            if (SpecificOperations.Contains(operation.ToCharArray()[0]))
                                CalculateResult();
                        }

                    }
                    else
                    {
                        if (firstValue.EndsWith(","))
                        {
                            firstValue = firstValue.Split(',')[0];
                            display = firstValue;
                        }
                        operation = Char.ToString(inPressedDigit); //This is the new operation

                        if (SpecificOperations.Contains(operation.ToCharArray()[0]))
                            CalculateResult();
                    }
                }
                else if (inPressedDigit.Equals(commaOperation))
                {
                    if (!(String.IsNullOrEmpty(operation)))
                        secondValue += Char.ToString(inPressedDigit); //Because the operation is filled already
                    else
                        firstValue += Char.ToString(inPressedDigit);
                }
                    
                else
                {
                    display = exception;
                    return;
                }
            }
            //throw new NotImplementedException();
        }

        private void CalculateResult()
        {
            String tmpDisplay = null;
            double result = 0;

            
            if(!String.IsNullOrEmpty(operation))
            {
                if (SpecificOperations.Contains(operation.ToCharArray()[0]))
                {
                    result = SpecificOperationCounter(operation.ToCharArray()[0], double.Parse(firstValue));
                    operation = null;
                }
                    
                else
                {
                    result = OperationCounter(operation.ToCharArray()[0], double.Parse(firstValue), double.Parse(secondValue));
                    secondValue = null;
                    operation = null; 
                }
                if (result < 1)
                    firstValue = result.ToString("G9");
                else
                    firstValue = result.ToString("G10");

            }
            else
            {
                display = exception;
                return;
            }

            if (Math.Abs(result) < 1)
                display = result.ToString("G9");
            else if (((int)Math.Abs(result) / maxDecimals < 10) && (Math.Abs(result) > 1))
                display = result.ToString("G10");
            else
                display = exception;

        }

        private double OperationCounter(char operation, double ValueF1, double ValueF2)
        {
            double result = 0;

            switch (operation)
            {
                case '+':
                    result = ValueF1 + ValueF2;
                    break;

                case '-':
                    result = ValueF1 - ValueF2;
                    break;

                case '*':
                    result = ValueF1 * ValueF2;
                    break;

                case '/':
                    result = ValueF1 / ValueF2;
                    break;
            }
            return result;
        }
        private double SpecificOperationCounter(char operation, double ValueF)
        {
            double result = 0;

            switch (operation)
            {
                case 'M':
                    result = ValueF * -1;
                    break;

                case 'S':
                    result = Math.Sin(ValueF);
                    break;

                case 'K':
                    result = Math.Cos(ValueF);
                    break;

                case 'T':
                    result = Math.Tan(ValueF);
                    break;

                case 'Q':
                    result = Math.Pow(ValueF, 2);
                    break;

                case 'R':
                    result = Math.Sqrt(ValueF);
                    break;

                case 'I':
                    result = 1 / ValueF;
                    break;

                case 'P':
                    if (String.IsNullOrEmpty(memory))
                        memory = ((int)ValueF).ToString();
                    else
                        memory += ((int)ValueF).ToString();
                    break;

                case 'G':
                    result = double.Parse(memory);
                    break;
            }

            return result;

        }

        private String ReturnFormatedString(String result)
        {
            double dispResult = double.Parse(result);
            String returningString;
            if (Math.Abs(dispResult) < 1)
                returningString = dispResult.ToString("G9");
            else if (double.IsInfinity(dispResult))
                returningString = exception;
            else if ((Math.Abs(dispResult) / maxDecimals < 10) && (Math.Abs(dispResult) >= 1))
                returningString = dispResult.ToString("G10");
            else
                returningString = exception;
            return returningString;
        }

        public string GetCurrentDisplayState()
        {
            return ReturnFormatedString(display);

        }
    }


}
