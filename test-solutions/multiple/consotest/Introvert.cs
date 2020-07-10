namespace introspection
{
    public class Outside
    {
        public static int Some()
        {
            return 1;
        }
        public class Inside
        {
            public static int Some()
            {
                return 1;
            }
            public class Within
            {
                public static int Some()
                {
                    return 1;
                }
                public class DepperWithin
                {
                    public static int Some()
                    {
                        return 1;
                    }
                }
            }
        }
    }
}