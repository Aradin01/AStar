using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AStar
{
   
    class Node //Trida uzlu pro spravnou funkcnost algoritmu.Kazdy uzel reprezentuje jedno policko.          
    {
        
        private List<Tile> path = null;//Cesta, po ktere bylo policko dosazeno. List policek. Nezacina startem, nybrz prvnim prozkoumanym polickem

        private Tile tile;//Policko dosazene prostrednictvim pat. Zaroven policko ve kterem se nachazime.

        private float backwardCost; //Cena z aktualniho policka do startu skrze path

        private float heuristicCost;//Nejmensi mozna cena z aktualniho policka do endu 

        /* 
         * Konstruktor Uzlu.
         * Uzel ma jako parametr
         * -> Policko ke kteremu patri - "tile"
         * -> Rodice(predchozi clen v path, pouze uzel!) - "parent",
         * -> Cenu prechodu z rodice do policka "transferCost"
         * -> Nejmensi moznou cenu za cestu z policka do endu "heuristicCost"
         */
        public Node(Tile tile, Node parent, float transferCost, float heuristicCost)
        {
            this.tile = tile;

            
            if (parent == null)//Pokud je tile startovni policko(pokud policko nema rodice). Zaroven inicializace path
            {
                backwardCost = 0;
                path = new List<Tile>();
            }
            else
            {
                //backwardCost predchoziho(rodicovskeho) policka + cena prechodu do aktualniho policka. To nam da idealni predpokladanou cestu
                backwardCost = parent.backwardCost + transferCost;                
                path = new List<Tile>(parent.path);//Pridani predchoziho(rodicovskeho) policka do path
                path.Add(tile);
            }

            this.heuristicCost = heuristicCost;
        }
       
        public float getCost() //Getter pro predpokladanou cenu cesty
        {
            return backwardCost + heuristicCost;
        }
      
        public Tile getTile() //Getter policka
        {
            return tile;
        }
       
        public List<Tile> getPath()//Getter path
        {
            return path;
        }
    }
    
    class AStar//Trida algoritmu
    {
        
        private const float DIAGONAL_TRANSFER_COST = 1.41421f;//Diagonalni cena prechodu mezi policky
        
        private const float HV_TRANSFER_COST = 1f;//Horizontalni a vertikalni cena prechodu mezi policky
        
        public static float Heuristic(Tile tile, Tile end)//Vypocet nejmensi mozne ceny z aktualniho policka do endu  
        {
           //Prevedeni na vektor
           float toEndX = Math.Abs(end.locX - tile.locX);
           float toEndY = Math.Abs(end.locY - tile.locY);
           //Odvozeni mensi a vetsi delky vektoru na osach
           float smaller = toEndX < toEndY ? toEndX : toEndY;
           float bigger = toEndX < toEndY ? toEndY : toEndX;

           /* 
            * Finalni vypocet ceny cesty.
            * Prvne vyradime ctverec tvoreny mensim vektorem "smaller * DIAGONAL_TRANSFER_COST"a nasledne pricteme rozdil vektoru "(bigger - smaller)* HV_TRANSFER_COST"
            * (rozdil vektoru je to, co zbyva k dosazeni finalniho konce,coz musime dosahnout horizontalne nebo vertikalne)
            */
           return smaller * DIAGONAL_TRANSFER_COST + (bigger - smaller)* HV_TRANSFER_COST;
        }

        public static void RunAStar(object o)//Startovni metoda A*
        {
            /* 
             * Koeficient chamtivosti algoritmu. Vysoká chamtivost algoritmu dává důraz na prioritu k přiblížení k cíli.
             * Pokud chamtivost zvyšíme příliš moc algoritmus se z A* mění na first search algorithm, který nehledá nejkratší cestu, nýbrž první možnou cestu.
             */
            float GREED_COEF = 1f; 
            Board board = (Board)o;

            
            List<Node> frontier = new List<Node>();//Seznam kandidatnich policek na prohledani
             
            List<Tile> resultPath = null;//Seznam policek vysledne cesty. Zacatek
            
            //Nacteni startu a endu z grafu
            Tile start = board.getStart();
            Tile end = board.getEnd();
                      
            Node min; //Uzel s nejmensi predpokladanou cenou cesty
            
            if (start == null)//Kontrola pritomnosti startu
            {
                MessageBox.Show("NENI START");
                Form1.UIEnabled = true; //A* skončil. Povolit UI pro uživatele.
                return;
            }
            
            if (end == null)//Kontrola pritomnosti endu
            {
                MessageBox.Show("NENI END");
                Form1.UIEnabled = true; // A* skončil. Povolit UI pro uživatele.
                return;
            }
            
            frontier.Add(new Node(start, null, 0, GREED_COEF * Heuristic(start, end)));//Pridani startovniho policka do fronty
           
            while (frontier.Count > 0)//Prohledavaci cyklus
            {
               
                min = GetMin(frontier); //Vyjmuti predpokladaneho nejvhodnejsiho policka ve fronte
                
                frontier.Remove(min);//Odstraneni vyjmuteho policka z fronty


                /*  
                 *  Pokud je vybrane policko cil, tak ulozit path a opustit cyklus
                 *  Diky operaci GetMin(frontier) se vybere kandidat s co nejmensi cenou za cestu. Diky tomu neni treba kontrolovat zda je cesta nejkratsi
                 */
                if (min.getTile().getState() == Tile.State.End) 
                {
                    resultPath = min.getPath();
                    break;
                }

               /*  
                *  Pokud neni policko Accessible nebo Start, tak preskocit.
                *  Fronta muze obsahovat vice instanci toho sameho policka s nekolika variacemi path.
                *  Operaci GetMin(frontier) se vybere predpokladany nejlepsi kandidat na prohledani,
                *  takze pokud policko jiz bylo prohledane(State == Expanded), nema cenu ho prohledavat znovu
                *  z duvodu vyssiho backwardCostu, kdyz bude heuristicCost stejne
                */
                if (min.getTile().getState() != Tile.State.Accessible && min.getTile().getState() != Tile.State.Start)
                {
                    continue;
                }

                
                if (min.getTile().getState() != Tile.State.Start)//Zmeni stav policka na prave prohledavane
                {
                    min.getTile().setState(Tile.State.Examined);
                }
                
                    System.Threading.Thread.Sleep(50);//Uspani vlakna z duvodu vizualizace postupu algoritmu

                /*
                 * Pridat sousedy do fronty
                 * Do fronty se pridavaji pouze policka pokud (State == Accessible || State == End)
                 * Policka jsou ve tvaru uzlu
                 */ 
                foreach (Tile neighbour in board.getNeighbours(min.getTile()))
                {
                    frontier.Add(new Node(neighbour, min, GetTransferCost(min.getTile(), neighbour), GREED_COEF*Heuristic(neighbour, end)));
                }
               
                if (min.getTile().getState() != Tile.State.Start) //Nastavit policko na prohledane
                {
                    min.getTile().setState(Tile.State.Expanded);
                }
                
                System.Threading.Thread.Sleep(15);//Uspani vlakna z duvodu vizualizace postupu algoritmu
            }
           
            if(resultPath == null)//Pokud se nenasla cesta
            {
                MessageBox.Show("CESTA NEBYLA NALEZENA");
                Form1.UIEnabled = true; // A* skončil. Povolit UI pro uživatele.
                return;
            }
        
            foreach (Tile tile in resultPath)//Zobrazeni vysledne cesty
            {
                if (tile.getState() != Tile.State.End)
                {
                    tile.setState(Tile.State.Path);
                }
            }

            Form1.UIEnabled = true; //A* skončil. Povolit UI pro uživatele.
        }
        
        public static Node GetMin(List<Node> frontier)//Vyjme policko s nejmensi Cost hodnotou
        {
            Node min = null;
            
            foreach (Node item in frontier)//Hledani policka s nejmensi Cost hodnotou
            {
                if(min == null)
                {
                    min = item;
                }
                else if(min.getCost() > item.getCost())
                {
                    min = item;
                }
            }
            return min;
        }
       
        public static float GetTransferCost(Tile tile, Tile neighbour) //Vypocet ceny prechodu z policka na souseda
        {
            //Převod na vektor
            float toNeighbourX = neighbour.locX - tile.locX;
            float toNeighbourY = neighbour.locY - tile.locY;
            float cost = 0;

            //Cena za horizontalni/vertikalni nebo diagonalni
            if (toNeighbourX == 0 || toNeighbourY == 0)
            {
                cost = 1;
            }
            else
            {
                cost = 1.41421f;
            }

            return cost;
        }

    }
}
