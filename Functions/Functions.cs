namespace ProjectFunctions
{
    public static class Functions
    {
        public static bool StringIsNumber(string inputString)
        {
            foreach(char symbol in inputString)
            {
                if(Char.IsLetter(symbol))
                {
                    return false;
                }
                else if(Char.IsPunctuation(symbol))
                {
                    return false;
                }
            }
            return true;
        }

        public static string EncryptKey(string inputKey)
        {
            string newKey = "";
            int keyLength = inputKey.Length;
            int lastSymbols = 20;
            
            if(keyLength <= 70)
            {
                return inputKey;
            }

            for (int i = 0; i < 10; i++)
            {
                newKey += "*";
            }

            for(int i = keyLength - lastSymbols; i < keyLength; i++)
            {
                newKey += inputKey[i];
            }

            return newKey;
        }
    }
}