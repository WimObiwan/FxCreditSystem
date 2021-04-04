using System;

namespace FxCreditSystem.Common.Fakers
{
   public static class ExtensionsForRandomizer
   {
      public static string Identity(this Bogus.Randomizer randomizer)
      {
         return $"test|{randomizer.Hexadecimal(16, "")}";
      }

      public static decimal Money(this Bogus.Randomizer randomizer, decimal min = 0.0m, decimal max = 1.0m)
      {
         return Math.Round(randomizer.Decimal(min, max), 9);
      }
   }
}