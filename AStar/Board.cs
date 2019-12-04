using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace AStar
{
   
    class Board : Panel //Trida grafu
    {
        
        private const int BOARD_SIZE = 25;//Pocet policek na stranu grafu
        
        Tile[,] tiles;//Pole policek v grafu

       /*
        * Oddelene promenne pro zapamatovani policka startu a endu
        * Je potreba, aby si graf pamatoval, kde je start a end a to z duvodu nasledneho predavani pozice startua endu do algoritmu A*.
        * Proto pro ne jsou udelany promenne a setter/getter.
        */
        Tile start;
        Tile end;

        
        public Board()//Konstruktor grafu
        {
            //Nastaveni lokace, velikosti grafu a incializace policek v grafu
            Location = new Point(12, 12);
            Size = new Size(800, 800);
            InitializeBoard();
        }

        
        private void InitializeBoard()//Inicializace policek v grafu
        {
            tiles = new Tile[BOARD_SIZE, BOARD_SIZE];//Inicializace pole policek
            
           
            for (int x = 0; x < BOARD_SIZE; x++) //Cyklovani radku
            {    
                
                for (int y = 0; y < BOARD_SIZE; y++)//Cyklovani sloupcu
                {
                    
                    tiles[x, y] = new Tile(x, y, Tile.State.Accessible, this);//Konstrukce policek. Zaroven pridavani do pole policek v grafu a pripisovani stavu
                    Controls.Add(tiles[x, y]);//Pridavani policka do UI
                }
            }
        }

       
        public void LoadMaze(int mazeIdx) //Metoda pro nacteni bludiste z build-in databaze
        {
            //Vyresetovani startu a endu grafu
            start = null;
            end = null;  
                       
            int tileidx = 0; //Pomocna promena pro prochazeni nacitaneho grafu v build-in databazi
           
            Tile.State currentState;//Pomocna promena pro ukladani noveho stavu policka
           
            if (mazeIdx < 6 && mazeIdx >= 0)//Podminka zda index grafu odpovida
            {
                for (int x = 0; x < BOARD_SIZE; x++)//Prochazeni sloupcu
                {
                    for (int y = 0; y < BOARD_SIZE; y++)//Prochazeni radku
                    {                        
                        currentState = (Tile.State)Mazes.mazes[mazeIdx, tileidx];//Ukladani noveho stavu policka

                        //Podminka pro nastaveni startu a endu
                        if (currentState == Tile.State.Start) setStart(tiles[x, y]);
                        else if (currentState == Tile.State.End) setEnd(tiles[x, y]);
                        else tiles[x, y].setState(currentState);
                       
                        tileidx++; //Posun na dalsi policko
                    }
                }

            }
        }


        
        public void setStart(Tile tile)//Setter pro start policko
        {
            
            if (start != null)//Osetreni stareho startu
            {
                start.setState(Tile.State.Accessible);
            }

            
            tile.setState(Tile.State.Start);//Nastaveni startu
            start = tile;
        }

       
        public Tile getStart()//Getter pro start policko
        {
            return start;
        }

        
        public void setEnd(Tile tile)//Setter pro end policko
        {
            
            if (end != null)//Osetreni stareho endu
            {
                end.setState(Tile.State.Accessible);
            }

            
            tile.setState(Tile.State.End);//Nastaveni endu
            end = tile;
        }

        //Getter pro end policko
        public Tile getEnd()
        {
            return end;
        }


        public List<Tile> getNeighbours(Tile tile)//Vraci list accessible sousedu policka
        {
            List<Tile> neighbours = new List<Tile>();

            
            for (int x = -1; x <= 1; x++)//sousedi jsou v rozmezi 1 policka od parametru tile na ose X a Y
            {
                for (int y = -1; y <= 1; y++)
                {                     
                    if (tile.locX + x < 25 && tile.locX + x > -1 && tile.locY + y < 25 && tile.locY + y > -1)//Osetreni zda jsme stale v grafu
                    {                        
                        Tile.State currentState = tiles[tile.locX + x, tile.locY + y].getState();//Ulozit stav
                        
                        //Pridani vsech policek se stavem Accesible a End(specialni forma accesible) do listu
                       if (currentState == Tile.State.Accessible || currentState == Tile.State.End)
                        {
                            neighbours.Add(tiles[tile.locX + x, tile.locY + y]);
                        }
                    }
                }
            }
            return neighbours;
        }

        
        public void ResetMaze()//Resetovani vsech policek se stavem Expanded, Examined nebo Path na stav Accesible
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {

                    Tile.State currentState = tiles[x, y].getState();
                    if (currentState == Tile.State.Examined || currentState == Tile.State.Expanded || currentState == Tile.State.Path)
                        tiles[x, y].setState(Tile.State.Accessible);
                }
            }
        }
    }
}
