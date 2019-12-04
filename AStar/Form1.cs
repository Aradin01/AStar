using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;


namespace AStar
{
    public partial class Form1 : Form
    {        
        Board board;//Graf        

        ListBox mazeList;//Seznam bludist

        Thread thread; //Predpripravene vlakno pro A*
         
        Button start;//Tlacitka pro spusteni
        Button reset;//Tlacitko pro reset bludiste
        Button help;//Tlacitko pro napovedu
        Button stop;//Tlacitko pro pohotove zastaveni A*
       
        public static bool UIEnabled = true;//Urcuje zda je UI ovladatelne
        
        public Form1()//incializace formulare, grafu a UI
        {
            InitializeComponent();
            board = new Board();
            Controls.Add(board);
            MenuInitialization();
        }
        
        private void MenuInitialization()//inicializace nabidky formulare
        {           
            start = new Button //inicializace tlacitka START
            {
                Location = new Point(824, 12),
                Size = new Size(152, 100),
                Text = "START"
            };
            start.Click += new EventHandler(start_Click);
            Controls.Add(start);
            
            reset = new Button//inicializace tlacitka RESET
            {
                Location = new Point(824, 124),
                Size = new Size(152, 100),
                Text = "RESET"
            };
            reset.Click += new EventHandler(reset_Click);
            Controls.Add(reset);        
            
            help = new Button//inicializace tlacitka HELP
            {
                Location = new Point(824, 236),
                Size = new Size(152, 100),
                Text = "HELP"
            };
            help.Click += new EventHandler(help_Click);
            Controls.Add(help);
            
            stop = new Button//inicializace tlacitka STOP
            {
                Location = new Point(824, 348),
                Size = new Size(152, 100),
                Text = "STOP"
            };
            stop.Click += new EventHandler(stop_Click);
            Controls.Add(stop);
            
            mazeList = new ListBox//incializace listu bludist
            {
                Location = new Point(824, 460),
                Size = new Size(152, 200)         
            };
            mazeList.SelectedIndexChanged += new EventHandler(mazeList_SelectedIndexChanged);
            Controls.Add(mazeList);
            mazeList.Items.Add("Sachovnice");
            mazeList.Items.Add("Oblicej");
            mazeList.Items.Add("Slozite bludiste");
            mazeList.Items.Add("Velke L");
            mazeList.Items.Add("Bez reseni");
            mazeList.Items.Add("Prazdno");

        }

        private void start_Click(object sender, EventArgs e)//metoda pro tlacitko START
        {
            if (UIEnabled)
            {
                UIEnabled = false;
                thread = new Thread(AStar.RunAStar);//Vytvoreni noveho vlakna, kde bude pracovat A*.
                thread.IsBackground = true;//Vlakno se nastavi na Background. To znamena, ze po ukonceni hlavniho vlakna(formular) se ukonci i toto vytvorene vlakno(A*)
                thread.Start(board);
            }
        }

        private void reset_Click(object sender, EventArgs e)//metoda pro tlacitko RESET
        {
            if (UIEnabled)
            {
                board.ResetMaze();
            }
        }
        private void help_Click(object sender, EventArgs e)//metoda pro tlacitko HELP
        {
            if (UIEnabled)
            {
                MessageBox.Show(
                                "Tlačítka:\n" +
                                "START \t\t-> Spusti A*\n" +
                                "RESET \t\t-> Vyresetuje bludiště po spuštění A*\n" +
                                "HELP \t\t-> Nápověda\n" +
                                "STOP \t\t-> Pohotové zastavení A*\n" +
                                "MAZELIST \t-> Seznam bludišť(po kliknutí se načte)\n\n" +
                                "Interakce s bludištěm:\n" +
                                "CLICK LMB \t\t-> Prázdno/Překážka\n" +
                                "DOUBLE-CLICK LMB \t-> Nastaví start\n" +
                                "DOUBLE-CLICK RMB \t-> Nastaví konec\n\n"                                
                );
            }
        }

        private void stop_Click(object sender, EventArgs e)//metoda pro tlacitko STOP
        {
            if(!UIEnabled)
            {
                thread.Abort();
                board.ResetMaze();
                UIEnabled = true;
            }
        }
        private void mazeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UIEnabled)
            {
                board.LoadMaze(mazeList.SelectedIndex);
            }
        }
    }
}
