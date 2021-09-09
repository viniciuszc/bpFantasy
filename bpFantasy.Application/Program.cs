using System;
using System.Collections.Generic;
using System.Net;
using bpFantasy.Domain.Entities;
using bpFantasy.Services;
using System.Linq;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace bpFantasy.Application
{
    class Program
    {
        static APIBPFantasy app = new APIBPFantasy();

        static void Main(string[] args)
        {

            int userInput = 0;
            do
            {
                userInput = DisplayMenu();
                
                //1. Listar times
                if (userInput == 1)
                {
                    Console.WriteLine("Aguarde ...");
                    DisplayTimes();
                }

                //2.Simular resultados a partir das médias dos times
                if (userInput == 2)
                {
                    Console.WriteLine("Aguarde ...");
                    Media timeMediaSelected = new Media();

                    do
                    {
                        timeMediaSelected = DisplaySelectTeamSim();
                    } while (timeMediaSelected.Nome.ToString() == "");

                    DisplaySimulation(timeMediaSelected);
                }

                //3.Médias dos times
                if (userInput == 3)
                {
                    Console.WriteLine("Aguarde ...");

                    Media TimesMedia = app.allTimesMediaDasMedias();

                    PrintAvg(TimesMedia);
                }

                //4.Selecione um time para comparar com a média da Liga
                if (userInput == 4)
                {
                    Console.WriteLine("Aguarde ...");

                    Media timeMediaSelected = new Media();
                    do
                    {
                        timeMediaSelected = DisplaySelectTeamSim();
                    } while (timeMediaSelected.Nome.ToString() == "");

                    Media timesMedia = app.allTimesMediaDasMedias();
                    CompareMedia(timeMediaSelected, timesMedia);

                }

                //5.Pontuaçao dos Jogadores do Time
                if (userInput == 5)
                {
                    Console.WriteLine("Aguarde ...");
                    DisplayTimes();
                    var idTimeJogadores = 0;
                    do
                    {
                        idTimeJogadores = Convert.ToInt32(Console.ReadLine());
                    } while (idTimeJogadores == 0);

                    DisplayJogadoresTime(idTimeJogadores);
                }

                //6.Melhor Escalação
                if (userInput == 6)
                {
                    Console.WriteLine("Aguarde ...");
                    DisplayTimes();
                    var idTimeJogadores = 0;
                    do
                    {
                        idTimeJogadores = Convert.ToInt32(Console.ReadLine());
                    } while (idTimeJogadores == 0);

                    DisplayMelhorEscalacao(idTimeJogadores);
                }

                //7. Melhores Jogadores
                if (userInput == 7)
                {
                    Console.WriteLine("Aguarde ...");
                    DisplayMelhoresJogadoresGenericos();
                }

                //8. Melhores Jogadores NBA API
                if (userInput == 8)
                {
                    Console.WriteLine("Aguarde ...");
                    DisplayMediaAPINBA();
                }

                //9. Melhores Free Agency
                if (userInput == 9)
                {
                    Console.WriteLine("Aguarde ...");
                    DisplayJogadoresSemTime();
                }
                
                //10 Exporta CSV
                if (userInput == 10)
                {
                    Console.WriteLine("Aguarde ...");
                    ExportCSVJogadores();
                }


            } while (userInput > 0);

            Console.ReadKey();
        }

        private static void ExportCSVJogadores()
        {
            int idTimeJogadores = 95;
            Time time = new Time();
            time = app.Time(idTimeJogadores);
            Console.WriteLine(time.Nome);
            PrintStaticHeader();

            Media mediaTimes = app.allJogadoreMedia();

            IList<Jogador> jogadoresTime = app.jogadoresTime(idTimeJogadores);

            Media[] ptsFinalJogadores = new Media[jogadoresTime.Count()];

            int i = 0;

            foreach (Jogador jogador in jogadoresTime)
            {
                IList<Media> mediasJogador = app.mediasJogador(jogador.Id);
                if (mediasJogador.Count() > 0)
                {
                    mediasJogador[0].PtsFinal = PontuacaoJogador(mediasJogador[0], mediaTimes);
                    ptsFinalJogadores[i] = mediasJogador[0];

                }
                else
                {
                    Media jogadorZerado = new Media();
                    jogadorZerado.Nome = jogador.Nome;
                    jogadorZerado.Pts = 0;
                    jogadorZerado.Reb = 0;
                    jogadorZerado.Ast = 0;
                    jogadorZerado.Blk = 0;
                    jogadorZerado.Stl = 0;
                    jogadorZerado.Tov = 0;
                    jogadorZerado.Pt3 = 0;
                    jogadorZerado.PtsFinal = 0;

                    ptsFinalJogadores[i] = jogadorZerado;
                }

                i++;
            }

            IEnumerable<Media> query = ptsFinalJogadores.OrderByDescending(j => j.PtsFinal);

            List<Media> mediaJogadoresCSV = query.ToList();

            using (var writer = new StreamWriter($"C:\\pessoal\\bpFantasy\\mediaJogadoresJund.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(mediaJogadoresCSV);
            }
        }

        static public int DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Fantasy Bola Presa"); ;
            Console.WriteLine();
            Console.WriteLine("1. Listar times");
            Console.WriteLine("2. Simular resultados a partir das médias dos times");
            Console.WriteLine("3. Médias dos times");
            Console.WriteLine("4. Selecione um time para comparar com a média da Liga");
            Console.WriteLine("5. Pontuaçao dos Jogadores do Time");
            Console.WriteLine("6. Melhor Escalação");
            Console.WriteLine("7. Melhores Jogadores");
            Console.WriteLine("8. Melhores Jogadores NBA API");
            Console.WriteLine("9. Melhores Free Agency");
            Console.WriteLine();
            Console.WriteLine("0. Sair");
            Console.WriteLine();
            var result = Console.ReadLine();
            return Convert.ToInt32(result);
        }

        static public void DisplayTimes()
        {
            IList<Time> Times = app.allTimes();

            Console.WriteLine("Listando todos os times");
            Console.WriteLine();
            foreach (Time time in Times)
            {
                Console.WriteLine(time.Id + ". " + time.Nome);
            }
        }

        static public Media DisplaySelectTeamSim()
        {
            IList<Media> TimesMedia = app.allTimesMedia();

            Console.WriteLine("Selecione um time:");
            Console.WriteLine();

            var i = 0;
            foreach (Media time in TimesMedia)
            {
                Console.WriteLine(i++ + ". " + time.Nome);
            }

            var result = Convert.ToInt32(Console.ReadLine());
            
            return TimesMedia[result];
        }

        static public void DisplaySimulation(Media timeMedia1)
        {
            IList<Media> mediaTimes = app.allTimesMedia();

            Console.WriteLine("Listando média times comparando com " + timeMedia1.Nome);
            Console.WriteLine();

            foreach (Media mediaTime in mediaTimes)
            {

                 if (mediaTime.Id != 22) {

                    CompareMedia(timeMedia1, mediaTime);

                }
            }
        }

        static public void CompareMedia(Media media1, Media media2)
        {
            var comparativo1 = 0;
            var comparativo2 = 0;

            if (media1.Pts > media2.Pts){ comparativo1++; } else if (media1.Pts < media2.Pts) { comparativo2++; }
            if (media1.Reb > media2.Reb){ comparativo1++; } else if (media1.Reb < media2.Reb) { comparativo2++; }
            if (media1.Ast > media2.Ast){ comparativo1++; } else if (media1.Ast < media2.Ast) { comparativo2++; }
            if (media1.Blk > media2.Blk){ comparativo1++; } else if (media1.Blk < media2.Blk) { comparativo2++; }
            if (media1.Stl > media2.Stl){ comparativo1++; } else if (media1.Stl < media2.Stl) { comparativo2++; }
            if (media1.Tov < media2.Tov){ comparativo1++; } else if (media1.Tov > media2.Tov) { comparativo2++; }
            if (media1.Pt3 > media2.Pt3){ comparativo1++; } else if (media1.Pt3 < media2.Pt3) { comparativo2++; }

            Media mediaDiff = new Media();
            mediaDiff.Nome = "Diferença %";
            mediaDiff.Pts = Math.Round((media1.Pts / media2.Pts) * 100, 3);
            mediaDiff.Reb = Math.Round((media1.Reb / media2.Reb) * 100, 3);
            mediaDiff.Ast = Math.Round((media1.Ast / media2.Ast) * 100, 3);
            mediaDiff.Blk = Math.Round((media1.Blk / media2.Blk) * 100, 3);
            mediaDiff.Stl = Math.Round((media1.Stl / media2.Stl) * 100, 3);
            mediaDiff.Tov = Math.Round((media2.Tov / media1.Tov) * 100, 3);
            mediaDiff.Pt3 = Math.Round((media1.Pt3 / media2.Pt3) * 100, 3);

            PrintStaticHeader();
            PrintAvg(media1);
            PrintAvg(media2);
            if (comparativo1 > comparativo2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            PrintAvg(mediaDiff);
            Console.WriteLine(media1.Nome + " " + comparativo1 + " x " + comparativo2 + " " + media2.Nome);
            Console.ResetColor();
        }

        private static void DisplayJogadoresTime(int idTimeJogadores)
        {
            Time time = new Time();
            time = app.Time(idTimeJogadores);
            Console.WriteLine(time.Nome);
            PrintStaticHeader();

            Media mediaTimes = app.allJogadoreMedia();

            IList<Jogador> jogadoresTime = app.jogadoresTime(idTimeJogadores);

            foreach (Jogador jogador in jogadoresTime)
            {
                IList<Media> mediasJogador = app.mediasJogador(jogador.Id);
                if (mediasJogador.Count() > 0)
                {
                    DisplayJogador(mediasJogador[0], mediaTimes);
                }
            }
        }

        private static void DisplayMelhorEscalacao(int idTimeJogadores)
        {
            Time time = new Time();
            time = app.Time(idTimeJogadores);
            Console.WriteLine(time.Nome);
            PrintStaticHeader();

            Media mediaTimes = app.allJogadoreMedia();

            IList<Jogador> jogadoresTime = app.jogadoresTime(idTimeJogadores);

            Media[] ptsFinalJogadores = new Media[jogadoresTime.Count()];

            int i = 0;

            foreach (Jogador jogador in jogadoresTime)
            {
                IList<Media> mediasJogador = app.mediasJogador(jogador.Id);
                if (mediasJogador.Count() > 0)
                {
                    mediasJogador[0].PtsFinal = PontuacaoJogador(mediasJogador[0], mediaTimes);
                    ptsFinalJogadores[i] = mediasJogador[0];
                    
                }
                else
                {
                    Media jogadorZerado = new Media();
                    jogadorZerado.Nome = jogador.Nome;
                    jogadorZerado.Pts = 0;
                    jogadorZerado.Reb = 0;
                    jogadorZerado.Ast = 0;
                    jogadorZerado.Blk = 0;
                    jogadorZerado.Stl = 0;
                    jogadorZerado.Tov = 0;
                    jogadorZerado.Pt3 = 0;
                    jogadorZerado.PtsFinal = 0;

                    ptsFinalJogadores[i] = jogadorZerado;
                }

                i++;
            }

            IEnumerable<Media> query = ptsFinalJogadores.OrderByDescending(j => j.PtsFinal);

            foreach(Media mediaJogador in query)
            {
                PrintAvg(mediaJogador);
                Console.WriteLine("Pontuaçao: " + mediaJogador.PtsFinal);
            }
        }

        private static void DisplayMelhoresJogadoresGenericos()
        {
            IList<Media> mediasJogadores = app.mediasJogadorGenerico();
            Media mediaTimes = app.allJogadoreMedia();

            foreach (Media mediasJogador in mediasJogadores)
            {
                mediasJogador.PtsFinal = PontuacaoJogador(mediasJogador, mediaTimes);
            }

            IEnumerable<Media> query = mediasJogadores.OrderByDescending(j => j.PtsFinal);

            foreach (Media mediaJogador in query)
            {
                Console.WriteLine(mediaJogador.Nome.PadRight(26, ' ') + ": " + mediaJogador.PtsFinal);
            }
        }

        private static void DisplayMediaAPINBA()
        {
            IList<Media> mediasJogadores = app.mediasJogadorNBAAPI(); 
            Media mediaTimes = app.allJogadoreMedia();

            foreach (Media mediasJogador in mediasJogadores)
            {
                mediasJogador.PtsFinal = PontuacaoJogador(mediasJogador, mediaTimes);
            }

            IEnumerable<Media> query = mediasJogadores.OrderByDescending(j => j.PtsFinal);

            foreach (Media mediaJogador in query)
            {
                Console.WriteLine(mediaJogador.Nome.PadRight(26, ' ') + ": " + mediaJogador.PtsFinal);
            }
            
        }

        private static void DisplayJogadoresSemTimeNBAAPI()
        {
            IList<Media> mediasJogadoresNBA = app.mediasJogadorNBAAPI();
            IList<Media> mediasJogadoresLiga = app.mediasJogadorGenerico();

            Media mediaTimes = app.allJogadoreMedia();

            foreach (Media mediasJogador in mediasJogadoresNBA)
            {
                mediasJogador.PtsFinal = PontuacaoJogador(mediasJogador, mediaTimes);
            }
             

            IEnumerable<Media> query = mediasJogadoresNBA.OrderByDescending(j => j.PtsFinal);

            foreach (Media mediaJogador in query)
            {
                if (mediasJogadoresLiga.Where(x => x.Nome == mediaJogador.Nome).Count() == 0) { 
                    Console.WriteLine(mediaJogador.Nome.PadRight(26, ' ') + ": " + mediaJogador.PtsFinal);
                }
            }

        }

        private static void DisplayJogadoresSemTime()
        {
            IList<Media> mediasJogadoresLiga = app.mediasJogadorLivre();

            Media mediaTimes = app.allJogadoreMedia();

            foreach (Media mediasJogador in mediasJogadoresLiga)
            {
                mediasJogador.PtsFinal = PontuacaoJogador(mediasJogador, mediaTimes);
            }


            IEnumerable<Media> query = mediasJogadoresLiga.OrderByDescending(j => j.PtsFinal);

            foreach (Media mediaJogador in query)
            {
                if(mediaJogador.Jogos > 5) { 
                    Console.WriteLine(mediaJogador.Nome.PadRight(26, ' ') + ": " + mediaJogador.PtsFinal + " - J " + mediaJogador.Jogos + " - M " + mediaJogador.Minutos );
                }
            }

            List<Media> mediaJogadoresFACSV = query.ToList();

            using (var writer = new StreamWriter($"C:\\pessoal\\bpFantasy\\mediaFreeAgency.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(mediaJogadoresFACSV);
            }

        }

        static public void DisplayJogador(Media media1, Media media2)
        {
            Media mediaDiff = new Media();
            mediaDiff.Nome = "Pontuação %";
            mediaDiff.Pts = Math.Round((media1.Pts / media2.Pts) * 100, 3);
            mediaDiff.Reb = Math.Round((media1.Reb / media2.Reb) * 100, 3);
            mediaDiff.Ast = Math.Round((media1.Ast / media2.Ast) * 100, 3);
            mediaDiff.Blk = Math.Round((media1.Blk / media2.Blk) * 100, 3);
            mediaDiff.Stl = Math.Round((media1.Stl / media2.Stl) * 100, 3);
            mediaDiff.Tov = Math.Round((media1.Tov / media2.Tov) * 100, 3);
            mediaDiff.Pt3 = Math.Round((media1.Pt3 / media2.Pt3) * 100, 3);

            PrintAvg(media1);
            Console.ForegroundColor = ConsoleColor.Green;
            PrintAvg(mediaDiff);

            decimal pontuacaoJogador = PontuacaoJogador(media1, media2);

            Console.WriteLine("Jogos: " + pontuacaoJogador);

            Console.WriteLine("Pontuacao Final: " + pontuacaoJogador);
            Console.ResetColor();
        }

        static public decimal PontuacaoJogador(Media media1, Media media2)
        {
            Media mediaDiff = new Media();
            mediaDiff.Nome = "Pontuação %";
            mediaDiff.Pts = Math.Round((media1.Pts / media2.Pts) * 100, 3);
            mediaDiff.Reb = Math.Round((media1.Reb / media2.Reb) * 100, 3);
            mediaDiff.Ast = Math.Round((media1.Ast / media2.Ast) * 100, 3);
            mediaDiff.Blk = Math.Round((media1.Blk / media2.Blk) * 100, 3);
            mediaDiff.Stl = Math.Round((media1.Stl / media2.Stl) * 100, 3);
            mediaDiff.Tov = Math.Round((media1.Tov / media2.Tov) * 100, 3);
            mediaDiff.Pt3 = Math.Round((media1.Pt3 / media2.Pt3) * 100, 3);

            decimal pontuacaoFinal =
                mediaDiff.Pts +
                mediaDiff.Reb +
                mediaDiff.Ast +
                mediaDiff.Blk +
                mediaDiff.Stl -
                mediaDiff.Tov +
                mediaDiff.Pt3;

            return Math.Round(pontuacaoFinal, 3);
        }

        static public void PrintStaticHeader()
        {
            Console.WriteLine(
                ("Time/Jogador").PadRight(30, ' ')
                + " | "
                + ("Pts").ToString().PadLeft(8, ' ')
                + " | "
                + ("Reb").ToString().PadLeft(8, ' ')
                + " | "
                + ("Ast").ToString().PadLeft(8, ' ')
                + " | "
                + ("Blk").ToString().PadLeft(8, ' ')
                + " | "
                + ("Stl").ToString().PadLeft(8, ' ')
                + " | "
                + ("Tov").ToString().PadLeft(8, ' ')
                + " | "
                + ("Pt3").ToString().PadLeft(8, ' ')
                + " | "
            );
        }

        static public void PrintAvg(Media media)
        {
            Console.WriteLine(
                (media.Nome).PadRight(30, ' ')
                + " | "
                + (media.Pts).ToString().PadLeft(8, ' ')
                + " | "
                + (media.Reb).ToString().PadLeft(8, ' ')
                + " | "
                + (media.Ast).ToString().PadLeft(8, ' ')
                + " | "
                + (media.Blk).ToString().PadLeft(8, ' ')
                + " | "
                + (media.Stl).ToString().PadLeft(8, ' ')
                + " | "
                + (-media.Tov).ToString().PadLeft(8, ' ')
                + " | "
                + (media.Pt3).ToString().PadLeft(8, ' ')
                + " | "
            );
        }
    }
}
