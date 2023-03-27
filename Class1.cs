using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CriticalWay
{
    internal class TreeGraph
    {
        TreeNode[] TreeNodes; //Узлы графа
        TreeNode[] TreeNodesForOptimalRange; //Вектор хранящий в себе узлы определившие оптимальную точку
        int SizeTreeNodesForOptimalRange = 0;
        int[,] MatrixPaths; //Матрица путей
        public TreeGraph(int[,] MatrixPaths) //Получает матрицу на вход и формирует по ней узлы
        {
            this.MatrixPaths = MatrixPaths;
            TreeNodes = new TreeNode[MatrixPaths.GetLength(0)];
            TreeNodesForOptimalRange = new TreeNode[MatrixPaths.GetLength(0)];
            FormGraph();
            NodesOptimalMarsh();
        }
        private void NodesOptimalMarsh() //Строит оптимальные маршруты для каждой точки
        {
            if (FindFinalNode() is not null)
            {
                TreeNodesForOptimalRange[SizeTreeNodesForOptimalRange++] = FindFinalNode();
                while (SizeTreeNodesForOptimalRange != TreeNodes.Length)
                {
                    foreach (var i in TreeNodes)
                    {
                        if (!(TreeNodesForOptimalRange.Contains(i)) && i.Nasledniki.All(u => TreeNodesForOptimalRange.Contains(u.Node)))
                        {
                            i.FindOptimalNode();
                            TreeNodesForOptimalRange[SizeTreeNodesForOptimalRange++] = i;
                        }
                    }
                }
                //Console.WriteLine("Оптимальные маршруты построенны");
            }
            else Console.WriteLine("Финальная точка отсутвует");
        }
        public int FindMinWay(out List<int> Marsh) //Для нахождения минимального маршрута
        {
            int LenghMarsh = (int)FindFirstNode().OptimalPath;
            Marsh = new List<int>();
            MakeMinWay(Marsh, FindFirstNode());
            return LenghMarsh;
        }
        private void MakeMinWay(List<int> Marsh, TreeNode Node) //Для построения маршрута
        {
            Marsh.Add(Node.ID_Node);
            if (Node.OptimalNextDot is null) return;
            MakeMinWay(Marsh, Node.OptimalNextDot);
        }
        private TreeNode? FindFirstNode() //Для нахождения первого узла цепочки
        {
            foreach (var i in TreeNodes) if (i.Parents.Count == 0) return i;
            return null;
        }
        private TreeNode? FindFinalNode() //Для нахождения последнего узла цепочки
        {
            foreach (var i in TreeNodes) if (i.Nasledniki.Count == 0) return i;
            return null;
        }
        private void FormGraph() //Создаёт узлы графа
        {
            for (int i = 0; i < MatrixPaths.GetLength(0); i++)
            {
                TreeNodes[i] = new TreeNode(i + 1);
            }
            for (int i = 0; i < MatrixPaths.GetLength(0); i++)
            {
                TreeNodes[i].FormRelation(TreeNodes, MatrixPaths);
            }
        }
    }
    internal class TreeNode
    {
        public int ID_Node { get; protected set; } //Номер вершины
        public List<TreeNode> Parents { get; private set; } = new List<TreeNode>(); //Отцовские элементы
        public List<Relation> Nasledniki { get; private set; } = new List<Relation>(); //наследники
        public TreeNode? OptimalNextDot { get; private set; } = null; //Оптимальная следующая точка
        public int? OptimalPath { get; private set; } = null; //Маршрут до оптимальной точки учитывающий последующий путь
        public struct Relation //Структура для описания отношений между узлами
        {
            public TreeNode Node { get; private set; } //Узел для создания связи
            int NodePath; //Путь до узла
            public int WNextNodePath { get; private set; } = 0; //Путь до следующей вершины учитывая последующие маршруты
            public void FuncWNextNodePath() //Функция для опредения пути до следующей вершины учитывая последующие маршруты
            {
                if (Node.OptimalPath is not null)
                {
                    WNextNodePath = NodePath + (int)Node.OptimalPath;
                }
                else WNextNodePath = NodePath;
            }
            public Relation(TreeNode NextNode, int NextNodePath)
            {
                this.Node = NextNode;
                this.NodePath = NextNodePath;
            }
        }
        public TreeNode(int ID_Node)
        {
            this.ID_Node = ID_Node;
        }
        public void FindOptimalNode() //Функция для нахождения следующей оптимальной точки
        {
            foreach (var i in Nasledniki)
            {
                i.FuncWNextNodePath();
                if (OptimalNextDot is null || (OptimalNextDot is not null && OptimalPath < i.WNextNodePath))
                {
                    OptimalNextDot = i.Node;
                    OptimalPath = i.WNextNodePath;
                }
            }
        }
        public void FormRelation(TreeNode[] Graph, int[,] MatrixPath) //Функция формирует связи узла с другими узлами из вектора Graph, используя матрицу путей MatrixPath
        {
            for (int j = 0; j < MatrixPath.GetLength(1); j++) if (MatrixPath[(ID_Node - 1), j] > 0)
                {
                    Nasledniki.Add(new Relation(Graph[j], MatrixPath[(ID_Node - 1), j]));
                    Graph[j].Parents.Add(this);
                }
        }
    }
}
