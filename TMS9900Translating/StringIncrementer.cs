namespace TMS9900Translating
{
    public static class StringIncrementer
    {
        public static string Increment(string sourceString)
        {
            var charArray = sourceString.ToCharArray();
            IncrementArray(charArray, charArray.Length - 1);
            return new string(charArray);
        }

        private static void IncrementArray(char[] charArray, int positionToUpdate)
        {
            var lastChar = charArray[positionToUpdate];
            if (lastChar == '9')
                lastChar = 'A';
            else if (lastChar == 'Z' || lastChar == 'z')
            {
                lastChar = '0';
                if (positionToUpdate > 0)
                    IncrementArray(charArray, positionToUpdate - 1);
            }
            else
                ++lastChar;
            charArray[positionToUpdate] = lastChar;
        }
    }
}
