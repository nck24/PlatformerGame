// See https://aka.ms/new-console-template for more information
using System;
using OpenTK.Mathematics;
using GameTest.AI_code;

namespace GameTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*int repetitons = 1;
            int positive = 0;
            for (int k = 0; k < repetitons; k++){
                NeuralNetwork test = new NeuralNetwork(4, 5, 4, 3);
                test.SetEndFuncton( value => MathF.Max(0f, value) );
                test.SetEndOutputFuncton( value => value );
                test.SetActionFunction( (value) => {
                    if (value > 0){
                        return true;
                    }else{
                        return false;
                    }
                });

                test.SetInputValues(new float[] {1f, .5f, .33f, .17f});
                test.CalcOutputs();
                //Console.WriteLine("Output value : {0}", test.outputs[0].OutputNodeValue());
                bool[] output = test.GetActions();
                for (int i = 0; i < output.Length; i++){
                    Console.WriteLine(output[i]);
                    if (output[i]){
                        positive ++;
                    }
                }
            }
            //Console.WriteLine("\nTrue : {0}     False : {1}", positive, repetitons - positive);
            Console.ReadLine();
            */


            


            bool run = true;
            Score score;
            Settings sett = new Settings(1.1f, 1.12f, 120, 300);
            while (run){
                using (Game game = new Game(sett, 500)){
                    game.Run();
                    score = game.score;
                }
                Thread.Sleep(100);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nGAME OVER\n");
                Console.ResetColor();
                //Display score
                Console.WriteLine("Your stats :");
                Console.WriteLine("   Score : {0}", score.GetScore() );
                Console.WriteLine("   Time : {0} s", score.GetElapsedSeconds() );
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine("\nPress r to play again ...");
                Console.ResetColor();
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                if (pressedKey.Key != ConsoleKey.R){
                    run = false;
                }
                Console.Clear();
            }
            
        }
    }
}