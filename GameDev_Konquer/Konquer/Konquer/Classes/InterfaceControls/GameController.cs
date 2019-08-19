using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.InterfaceControls {
    class GameController {
        public bool LevelFinished { get; set; }
        public bool PlayerDeath { get; set; }

        public int MaxScoreCount { get; set; }

        public int CurrentLevel { get; set; }
        private int _scoreCount;
        public int ScoreCount { 
            get { return _scoreCount; }
            set {
                if (value > MaxScoreCount) {
                    Console.WriteLine("MaxScoreCount has been reached, act accordingly");
                    FinishLevel();
                }
                _scoreCount = value;
            }
        }

        public void FinishLevel() {
            if (!LevelFinished) {
                LevelFinished = true;
                Console.WriteLine("Level finished");
            }
        }



        private GameController() { }
        private static GameController instance;
        public static GameController Instance {
            get {
                if (instance == null)
                    instance = new GameController();
                return instance;
            }
        }

      

    }
}
