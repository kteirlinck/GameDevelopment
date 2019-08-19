using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.InterfaceControls {
    // De GameController klasse voorziet de blauwdruk voor de singleton die de score/progressie bijhoudt en controle heeft over de progressie binnen het spel.
    class GameController {
        private int _scoreCount;
        public bool LevelFinished { get; set; }
        public bool PlayerDeath { get; set; }

        public int MaxScoreCount { get; set; }

        public int CurrentLevel { get; set; }
        public int ScoreCount { 
            get { return _scoreCount; }
            set {
                if (value > MaxScoreCount) {
                    FinishLevel();
                }
                _scoreCount = value;
            }
        }

        public void FinishLevel() {
            if (!LevelFinished) {
                LevelFinished = true;
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
