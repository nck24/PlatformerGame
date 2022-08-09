using System;
using System.Diagnostics;

namespace GameTest
{
    public class Score
    {
        private int score;
        private long ms;
        private Stopwatch sw;
        
        public Score(){
            this.sw = new Stopwatch();
            this.score = 0;
        }

        public Score(int startScore){
            this.sw = new Stopwatch();
            this.score = startScore;
        }

        public void StartTimer(){
            this.sw.Start();
        }

        public void StopTimer(){
            this.sw.Stop();
        }

        public void ResetTimer(){
            this.sw.Reset();
        }

        public void SaveElapsedTime(){
            this.ms = sw.ElapsedMilliseconds;
        }

        public double GetElapsedMiliseconds(){
            return (double)ms;
        }

        public double GetElapsedSeconds(){
            return GetElapsedMiliseconds() / 1000d;
        }

        public int GetScore(){
            return score;
        }

        public void IncrementScore(int increment){
            this.score += increment;
        }

        public void IncrementScoreBy1(){
            this.score ++;
        }
    }
}