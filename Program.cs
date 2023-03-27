namespace CriticalWay
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите количество вершин графа");
            int k = Convert.ToInt32(Console.ReadLine());
            int[,] MasVersh = new int[k, k];
            for (int i = 0; i < k; i++)
            {
                int j = 0;
                foreach (var l in Array.ConvertAll(Console.ReadLine().Split(" "), str => Convert.ToInt32(str)))
                {
                    MasVersh[i, j++] = l;
                }
            }
            CriticalWay Zad = new CriticalWay(MasVersh);
        }
        class CriticalWay
        {
            TreeGraph Graph;
            public CriticalWay(int[,] MasVersh /*Массив отношений вершин*/)
            {
                Graph = new TreeGraph(MasVersh); //Создаём массив на введённое количество вершин
                List<int> minWay;
                Console.WriteLine($"Длина критического пути: {Graph.FindMinWay(out minWay)}");
                Console.Write("Критический маршрут: ");
                foreach (var i in minWay)
                {
                    Console.Write($"{i} ");
                }
            }
        }
    }
}