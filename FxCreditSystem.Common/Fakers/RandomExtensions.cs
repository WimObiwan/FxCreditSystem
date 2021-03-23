namespace FxCreditSystem.Common.Fakers
{
   public static class ExtensionsForRandomizer
   {
      public static string Identity(this Bogus.Randomizer randomizer)
      {
         return $"test|{randomizer.Hexadecimal(16, "")}";
      }
   }
}