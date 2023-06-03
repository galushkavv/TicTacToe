using System.ComponentModel;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private const int cells = 3;
        private int playerSymbol = Symbol.O;

        private List<List<int>> field = new List<List<int>>();
        private Label[,] labels = new Label[cells, cells];

        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < cells; i++)
                for (int j = 0; j < cells; j++)
                {
                    labels[i, j] = new Label();
                    labels[i, j].Font = new Font(FontFamily.GenericMonospace, 60, FontStyle.Bold);
                    labels[i, j].Width = 200;
                    labels[i, j].Height = 200;
                    labels[i, j].Text = " ";
                    labels[i, j].BorderStyle = BorderStyle.FixedSingle;
                    labels[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    labels[i, j].Top = 10 + i * labels[i, j].Width;
                    labels[i, j].Left = 10 + j * labels[i, j].Height;
                    labels[i, j].Tag = i.ToString() + j.ToString();
                    labels[i, j].Click += label_Click;

                    Controls.Add(labels[i, j]);
                }

            StartNewGame();
        }

        private void StartNewGame()
        {
            field = new List<List<int>> { new List<int> {0,0,0},
                                          new List<int> {0,0,0},
                                          new List<int> {0,0,0} };

            playerSymbol = Symbol.Not(playerSymbol);

            foreach (Control c in Controls)
            {
                if (c.GetType() == typeof(Label))
                    c.Text = " ";
            }

            if (playerSymbol == Symbol.O)
            {
                MakeComputerTurn(Symbol.Not(playerSymbol));
            }

        }

        private void label_Click(object sender, EventArgs e)
        {
            ((Label)sender).Text = playerSymbol == Symbol.X ? "X" : "O";

            for (int i = 0; i < cells; i++)
                for (int j = 0; j < cells; j++)
                {
                    if (sender.Equals(labels[i, j]))
                    {
                        field[i][j] = playerSymbol;
                        break;
                    }
                }

            int winner = CheckWin(field);
            if (winner == 0) // ���� ������������
            {
                MakeComputerTurn(Symbol.Not(playerSymbol));
            }

            winner = CheckWin(field);
            if (winner == playerSymbol)
            {
                MessageBox.Show("������");
                StartNewGame();
            }
            else if (winner == Symbol.Not(playerSymbol))
            {
                MessageBox.Show("���������");
                StartNewGame();
            }
            else if (winner == 3)
            {
                MessageBox.Show("�����");
                StartNewGame();
            }
        }

        private void MakeComputerTurn(int symbol)
        {
            int bestScore = int.MinValue;
            int bestI = 0, bestJ = 0;
            for (int i = 0; i < cells; i++)
                for (int j = 0; j < cells; j++)
                {
                    if (field[i][j] == Symbol.Empty)
                    {
                        field[i][j] = symbol;
                        int score = Minimax(field, symbol);
                        field[i][j] = Symbol.Empty;
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestI = i;
                            bestJ = j;
                        }
                    }
                }

            field[bestI][bestJ] = symbol;
            labels[bestI, bestJ].Text = symbol == Symbol.X ? "X" : "O";
        }

        private int Minimax(List<List<int>> field, int symbol, bool maximize = false)
        {
            int winner = CheckWin(field);

            if (winner == symbol)
                return 10;
            else if (winner == Symbol.Not(symbol))
                return -10;
            else if (winner == 3)
                return 0;

            int bestScore = maximize ? int.MinValue : int.MaxValue;

            for (int i = 0; i < cells; i++)
                for (int j = 0; j < cells; j++)
                {
                    if (field[i][j] == Symbol.Empty)
                    {
                        field[i][j] = maximize ? symbol : Symbol.Not(symbol);
                        int score = Minimax(field, symbol, !maximize);
                        field[i][j] = Symbol.Empty;
                        bestScore = maximize ? Math.Max(bestScore, score) : Math.Min(bestScore, score);
                    }
                }

            return bestScore;
        }

        /// <summary>
        /// ���������, �� ������� �� ���-������
        /// </summary>
        /// <param name="field">������� ����</param>
        /// <returns>1 - �������� �������� 2 - �������� ������; 3 - �����; 0 - ���� ����� ����������</returns>
        private int CheckWin(List<List<int>> field)
        {
            // �������� �� �������
            for (int i = 0; i < cells; i++)
            {
                if (field[i].Distinct().Count() == 1 && field[i][0] != Symbol.Empty)
                    return field[i][0];
            }

            // �������� �� ��������
            for (int i = 0; i < cells; i++)
            {
                if (field.Select(x => x[i]).Distinct().Count() == 1 && field[0][i] != Symbol.Empty)
                    return field[0][i];
            }

            // �������� �� ����������
            // ������� ���������
            //if (field.Where((subList, index) => index < subList.Count)
            //         .Select((subList, index) => subList[index])
            //         .Distinct().Count() == 1 && field[0][0] != Symbol.Empty)
            //    return field[0][0];
            
            if (field[0][0] != 0)
            {
                int val = field[0][0];
                int count = 1;
                for (int i = 1; i < cells; i++)
                {
                    if (field[i][i] == val)
                    {
                        count++;
                    }
                }
                if (count == cells)
                    return val;
            }

            // �������� ���������
            //if (field.Where((subList, index) => field.Count - 1 - index < subList.Count)
            //         .Select((subList, index) => subList[field.Count - 1 - index])
            //         .Distinct().Count() == 1 && field[0][0] != Symbol.Empty)
            //    return field[field.Count - 1][0];
            
            if (field[0][cells - 1] != 0)
            {
                int val = field[0][cells - 1];
                int count = 1;
                for (int i = 1; i < cells; i++)
                {
                    if (field[i][cells - 1 - i] == val)
                    {
                        count++;
                    }
                }
                if (count == cells)
                    return val;
            }

            // �������� ������
            if (field.All(subList => subList.All(x => x != Symbol.Empty)))
                return 3;

            return 0;
        }

    }

    public static class Symbol
    {
        public const int Empty = 0;
        public const int X = 1;
        public const int O = 2;

        public static int Not(int symbol)
        {
            return symbol == X ? O : X;
        }
    }
}