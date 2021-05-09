
namespace Cube
{
    public static class Center
    {
        public static int Default;
        private readonly static int[] inverseMap = { 0, 3, 2, 1, 12, 23, 6, 17,
            8, 9, 10, 11, 4, 19, 14, 21, 20, 7, 18, 13, 16, 11, 22, 5};
        private readonly static int[,] turnMap = {
            {3, 12, 16}, {0, 17, 5}, {1, 6, 22}, {2, 23, 15},
            {7, 0, 19}, {4, 16, 9}, {5, 10, 23}, {6, 20, 3},
            {11, 4, 18}, {8, 19, 13}, {9, 14, 20}, {10, 21, 7},
            {15, 8, 17}, {12, 18, 1}, {13, 2, 21}, {14, 22, 11},
            {19, 15, 10}, {16, 11, 6}, {17, 7, 2}, {18, 3, 14},
            {23, 13, 0}, {20, 1, 4}, {21, 5, 8}, {22, 9, 12}};
        private readonly static int[,] faceMap = {
            {0, 1, 2}, {0, 5, 1}, {0, 4, 5}, {0, 2, 4},
            {2, 1, 3}, {2, 0, 1}, {2, 4, 0}, {2, 3, 4},
            {3, 1, 5}, {3, 2, 1}, {3, 4, 2}, {3, 5, 4},
            {5, 1, 0}, {5, 3, 1}, {5, 4, 3}, {5, 0, 4},
            {4, 0, 2}, {4, 5, 0}, {4, 3, 5}, {4, 2, 3},
            {1, 3, 2}, {1, 5, 3}, {1, 0, 5}, {1, 2, 0}};
        public static int Inverse(int center)
        {
            return inverseMap[center];
        }
        public static int FaceAt(int center, int index)
        {
            return (faceMap[center, index % 3] + index / 3 * 3) % 6;
        }
        public static int Turn(int center, int face, int times)
        {
            for (int i = 0; i < times; i++)
                center = turnMap[center, face];
            return center;
        }
    }
}
