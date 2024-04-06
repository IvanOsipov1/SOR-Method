using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOR_Method_SLAE
{
    abstract class Matrix
    {
        public double[,] matrix;
        public int sizeMatrix;

        public virtual void matrixInputFromFile(string path)
        {
            try
            {
                // Путь к файлу для чтения

                string[] Lines = File.ReadAllLines(path);
                int rowCount = Lines.Length;
                int columnCount = Lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                sizeMatrix = rowCount;
                if (rowCount != columnCount)
                {
                    Console.WriteLine("Матрица не является квадратной. ");
                    Console.WriteLine("Программа завершена. ");
                    Environment.Exit(0);

                }

                matrix = new double[rowCount, rowCount];

                for (int i = 0; i < rowCount; i++)
                {
                    string[] values = Lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < columnCount; j++)
                    {
                        matrix[i, j] = Convert.ToDouble(values[j]);
                    }
                }


            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка при чтении файла: " + e.Message);

            }

        }

        public virtual void matrixInput()
        {
            Console.WriteLine("Введите размер квадратной матрицы: ");
            sizeMatrix = Convert.ToInt32(Console.ReadLine());
            matrix = new double[sizeMatrix, sizeMatrix];

            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    Console.WriteLine("Введите " + (i + 1) + " " + (j + 1) + " " + " элемент матрицы");
                    matrix[i, j] = Convert.ToDouble(Console.ReadLine());
                }
            }
        }

        public virtual void printMatrix()
        {
            Console.WriteLine("Ввееденая матрица: ");
            for (int i = 0; i < sizeMatrix; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine();
                }

                for (int j = 0; j < sizeMatrix; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
            }
            Console.WriteLine();

        }


    }

    class MainMatrix : Matrix
    {
        public void convergenceOfMethods() // проверка на сходимость
        {
            double[,] predominanceOfDiagonalElements = new double[sizeMatrix, 2];
            double sumOfRow = 0;
            int countDiag = 0;
            for (int i = 0; i < sizeMatrix; i++)
            {
                sumOfRow = 0;
                for (int j = 0; j < sizeMatrix; j++)
                {
                    sumOfRow += Math.Abs(matrix[i, j]);

                    if (i == j)
                    {
                        sumOfRow -= Math.Abs(matrix[i, j]);

                        predominanceOfDiagonalElements[i, 0] = Math.Abs(matrix[i, j]);
                    }
                    predominanceOfDiagonalElements[i, 1] = sumOfRow;
                }
            }

            for (int i = 0; i < sizeMatrix; i++)
            {
                if (predominanceOfDiagonalElements[i, 0] >= predominanceOfDiagonalElements[i, 1])
                {
                    countDiag++;
                }
            }
            if (countDiag == sizeMatrix)
            {
                Console.WriteLine("Итерационные методы сходятся. ");
            }
            else
            {
                Console.WriteLine("Нет сходимости итерационных методов, возможно, некоторые методы сходится не будут.");
                Console.WriteLine("Рекомендуется преобразовать матрицу и ввести ее снова.");
            }
        }

        public override void matrixInputFromFile(string path)
        {
            base.matrixInputFromFile(path);
        }

    }

    class FreeMatrix : Matrix
    {
        private int rowCount;
        private int columnCount;
        public override void matrixInputFromFile(string path)
        {
            try
            {
                // Путь к файлу для чтения

                string[] Lines = File.ReadAllLines(path);
                rowCount = Lines.Length;
                columnCount = Lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                sizeMatrix = rowCount;

                matrix = new double[rowCount, columnCount];

                for (int i = 0; i < rowCount; i++)
                {
                    string[] values = Lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < columnCount; j++)
                    {
                        matrix[i, j] = Convert.ToDouble(values[j]);
                    }
                }


            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка при чтении файла: " + e.Message);

            }
        }

        public override void printMatrix()
        {
            Console.WriteLine("Ввееденая матрица: ");
            for (int i = 0; i < rowCount; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine();
                }

                for (int j = 0; j < columnCount; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
            }
            Console.WriteLine();
        }
        public void matrixVector(int sizeMatrix)
        {

            Console.WriteLine("Введите вектор свободных значений");

            matrix = new double[sizeMatrix, 1];

            for (int i = 0; i < sizeMatrix; i++)
            {
                {
                    Console.WriteLine("Введите " + (i + 1) + " элемент вектора");
                    matrix[i, 0] = Convert.ToDouble(Console.ReadLine());
                }
            }
        }
    }

    class SLAE
    {

        MainMatrix mainMatrix = new MainMatrix();
        FreeMatrix freeMembers = new FreeMatrix();

        private double[] seidelValues;
        private double[] newUnknownValues;
        private double[] errors;


        private double errorValue;
        private double relaxParameter;
        public SLAE(MainMatrix matrix, FreeMatrix freeMembers, double errorValue, double relaxParameter)
        {
            this.mainMatrix = matrix;
            this.freeMembers = freeMembers;
            seidelValues = new double[matrix.sizeMatrix];
            newUnknownValues = new double[matrix.sizeMatrix];
            this.errorValue = errorValue;
            this.relaxParameter = relaxParameter;
        }
        public void SORMethod()
        {
            errors = new double[mainMatrix.sizeMatrix];
            mainMatrix.convergenceOfMethods();
            for (int i = 0; i < mainMatrix.sizeMatrix; i++) // находим первое прближение по формуле xi = bi/aii
            {
                errors[i] = 1;
                //newUnknownValues[i] = freeMembers.matrix[i, 0] / mainMatrix.matrix[i, i];
                newUnknownValues[i] = 0;

            }
            double b1 = 0;
            int operationCounter = 0;
            while (Math.Abs(errors.Max()) > errorValue)
            {
                for (int i = 0; i < mainMatrix.sizeMatrix; i++)
                {
                    b1 = 0;
                    for (int j = 0; j < mainMatrix.sizeMatrix; j++)
                    {
                        if (i != j)
                        {
                            b1 += mainMatrix.matrix[i, j] * seidelValues[j];

                        }

                    }
                    b1 = freeMembers.matrix[i, 0] - b1;

                    seidelValues[i] = b1 / mainMatrix.matrix[i, i];
                    newUnknownValues[i] = relaxParameter * seidelValues[i] + (1 - relaxParameter) * newUnknownValues[i];
                    
                }

                for(int i = 0; i < mainMatrix.sizeMatrix; i++)
                {
                    errors[i] = Math.Abs(seidelValues[i] - newUnknownValues[i]) / Math.Abs(newUnknownValues[i]);
                    seidelValues[i] = newUnknownValues[i];
                }
                operationCounter++;
                if (operationCounter > 200)
                {
                    Console.WriteLine("Метод не может быть применен для решения СЛАУ, скорее всего, погрешность перестала меняться.");
                    Console.WriteLine("Программа принудительно завершена");
                    Environment.Exit(0);
                }
            }



            for (int i = 0; i < mainMatrix.sizeMatrix; i++)
            {

                Console.WriteLine(seidelValues[i]);
            }
            Console.WriteLine("Количество итераций: " + operationCounter);

        }
    }
    internal class Program
    {


        static void Main(string[] args)
        {
            int choice1;
            int choice2;
            int choice3;
            int choice4;


            double error = 0.01;
            double relaxParameter = 0.5;

            MainMatrix matrix1 = new MainMatrix();
            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Ввести матрицу вручуню");
            Console.WriteLine("2. Ввести матрицу из файла Matrix.txt");
            choice1 = Convert.ToInt32(Console.ReadLine());
            if (choice1 == 1)
            {
                matrix1.matrixInput();
            }
            if (choice1 == 2)
            {
                matrix1.matrixInputFromFile(@"C:\study\SOR-Method\SOR-Method-SLAE\Matrix.txt");
            }

            matrix1.printMatrix();

            FreeMatrix matrix2 = new FreeMatrix();
            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Ввести матрицу вручуню");
            Console.WriteLine("2. Ввести матрицу из файла Matrix.txt");
            choice2 = Convert.ToInt32(Console.ReadLine());
            if (choice2 == 1)
            {
                matrix2.matrixVector(matrix1.sizeMatrix);
            }
            if (choice2 == 2)
            {
                matrix2.matrixInputFromFile(@"C:\study\SOR-Method\SOR-Method-SLAE\FreeMembers.txt");
            }

            matrix2.printMatrix();

            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Выбрать стандартную погрешность - 0.01");
            Console.WriteLine("2. Ввести другую погрешность");
            choice3 = Convert.ToInt32(Console.ReadLine());
            if (choice3 == 1)
            {
                error = 0.01;
            }
            if (choice3 == 2)
            {
                Console.WriteLine("Введите погрешность (Рекомендуется использовать запятую вместо точки при вводе числа): ");
                error = Convert.ToDouble(Console.ReadLine());
            }

            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Выбрать стандартный параметр релаксации ω - 0.5 ");
            Console.WriteLine("2. Ввести другой параметр релаксации");
            choice4 = Convert.ToInt32(Console.ReadLine());
            if (choice4 == 1)
            {
                relaxParameter = 0.5;
            }
            if (choice4 == 2)
            {
                Console.WriteLine("Введите параметр релаксации (Рекомендуется использовать запятую вместо точки при вводе числа): ");
                error = Convert.ToDouble(Console.ReadLine());
            }
            SLAE slae = new SLAE(matrix1, matrix2, error, relaxParameter);
            slae.SORMethod();

        }
    }
}
