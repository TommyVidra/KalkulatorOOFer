﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrvaDomacaZadaca_Kalkulator
{
    public class Factory
    {
        public static ICalculator CreateCalculator()
        {
            // vratiti kalkulator
            return new Kalkulator();
        }
    }

    public class Kalkulator:ICalculator
    {
        public void Press(char inPressedDigit)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentDisplayState()
        {
            throw new NotImplementedException();
        }
    }


}
