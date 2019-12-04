using System.Windows.Forms;
using System.Drawing;


namespace AStar
{
    class Tile : Panel
    {        
        private const int size = 30; //Velikost strany policka v pixelech. Potrebne pouze v nastaveni UI

       /*
        *  Stavy policka
        *  0 = Obstacle = Prekazka. Na policko neni mozne vstoupit, ani ho prozkoumavat
        *  1 = Acecesicle = Prazdne policko. Policko lze prozkoumat i na něj vstoupit
        *  2 = Path = Policko, ktere je soucasti vysledne cesty.  
        *  3 = Expanded = Prozkoumane policko.
        *  4 = Examined = Prave prozkoumavane policko. Zde se nachazi algoritmus
        *  5 = Start = Startovni policko algoritmu. Tuto pozici prozkoumava algoritmus jako prvni
        *  6 = End = Zaverecne policko algoritmu. Tuto pozici algoritmus hleda
        */
        public enum State {Obstacle = 0, Accessible = 1, Path = 2, Expanded = 3, Examined = 4, Start = 5, End = 6 };
      
        private State tileState; //Stav policka

        //Index  X, Y lokace. Souradnice policka na ose X a Y
        public int locX;
        public int locY;
       
        private Board board;//Graf, kde se policko nachazi
     
        public Tile(int locX, int locY, State tileState, Board board)//Konstruktor pro policko
        {            
            Location = new Point(locY * size, locX * size);//Nastaveni lokace v UI. POZOR: prohozeni Y a X souradnice z duvodu generovani tlacitek po radcich nikoliv po sloupcich
            
            Size = new Size(size, size);//Nastaveni velikosti. O trochu mensi nez velikost v kodu z duvodu vytvoreni mrizky

            setState(tileState);
            this.locX = locX;
            this.locY = locY;
            this.board = board;

            //inicializace Eventhandleru pro obsluhu MouseClick a MouseDoubleclik metody pro policka
            MouseClick += new MouseEventHandler(Tile_MouseClick);
            MouseDoubleClick += new MouseEventHandler(Tile_MouseDoubleClick);
        }
        
               
        private void Tile_MouseClick(object sender, MouseEventArgs e)//Event metoda menici stav policka mezi prekazkou a volnym prostorem   
        {
            if (Form1.UIEnabled)
            {                
                if (e.Button == MouseButtons.Left)//Podminkou kontrolujeme, zda bylo stisknuto leve tlacitko mysi a pouze jednou
                {
                    if (tileState == State.Obstacle) setState(State.Accessible);
                    else if (tileState == State.Accessible) setState(State.Obstacle);
                }
            }
        }      
        private void Tile_MouseDoubleClick(object sender, MouseEventArgs e) //Event metoda pro nastaveni stavu policka na start a end
        {
            if (Form1.UIEnabled)
            {                
                if (e.Button == MouseButtons.Left)//DoubleClick leveho tlacitka nastavi start
                {
                    board.setStart(this);
                }
                
                else if (e.Button == MouseButtons.Right)//DoubleClick praveho tlacitka nastavi end
                {
                    board.setEnd(this);
                }
            }
        }
        
        public void setState(State tileState)//Setter pro stav policka. Pro vizualizaci se nastavi barva pozadi policka
        {
            switch(tileState)
            {
                case (State.Obstacle):
                    BackColor = Color.Black;
                    break;
                case (State.Accessible):
                    BackColor = Color.White;
                    break;
                case (State.Path):
                    BackColor = Color.Blue;
                    break;
                case (State.Expanded):
                    BackColor = Color.OrangeRed;
                    break;
                case (State.Examined):
                    BackColor = Color.Aqua;
                    break;
                case (State.Start):
                    BackColor = Color.Yellow;
                    break;
                case (State.End):
                    BackColor = Color.Purple;
                    break;
            }
            this.tileState = tileState;
        }
        
        public State getState()//getter pro stav policka
        {
            return tileState;
        }


    }
}
