using System;
using System.Collections.Generic;
using System.Net;
using bpFantasy.Domain.Entities;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bpFantasy.Services
{
    public class APIBPFantasy
    {
        public static string url = @"http://bpfantasy.com.br/api/api.php";

        public static string getAPI(string parameters)
        {
            string response;

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                response = wc.UploadString(url, parameters);
            }

            return response;
        }

        public Time Time(int idTime)
        {
            string paramenters = "{\"class\":\"time\",\"action\":\"buscar\",\"params\":{\"timeId\":" + idTime + ",\"ligaId\":\"4\"}}";

            string response = getAPI(paramenters);

            JObject objTimes = JObject.Parse(response);

            Time time = objTimes["time"].ToObject<Time>();


            return time;
        }

        public IList<Time> allTimes()
        {
            string paramenters = "{\"class\":\"time\",\"action\":\"buscar\",\"params\":{\"timeId\":\"73\",\"ligaId\":\"4\"}}";

            string response = getAPI(paramenters);

            JObject objTimes = JObject.Parse(response);
            JArray arrTimes = (JArray)objTimes["times"];

            IList<Time> times = arrTimes.ToObject<IList<Time>>();

            return times;
        }

        public IList<Media> allTimesMedia()
        {
            string paramenters = "{\"class\":\"estatisticas\",\"action\":\"buscarEstatisticasTimesMedia\",\"params\":{\":liga_id\":\"4\",\":temporada_id\":\"3\"}}";

            string response = getAPI(paramenters);

            JArray objTimes = JArray.Parse(response);
            IList<Media> mediaTimes = objTimes.ToObject<IList<Media>>();

            return mediaTimes;
        }

        public Media allTimesMediaDasMedias()
        {
            IList<Media> mediaTimes = allTimesMedia();
            Media mediasDaMediasTime = new Media();

            mediasDaMediasTime.Nome = "Média de todos os Times";

            foreach (Media mediaTime in mediaTimes)
            {
                mediasDaMediasTime.Pts += mediaTime.Pts;
                mediasDaMediasTime.Reb += mediaTime.Reb;
                mediasDaMediasTime.Ast += mediaTime.Ast;
                mediasDaMediasTime.Blk += mediaTime.Blk;
                mediasDaMediasTime.Stl += mediaTime.Stl;
                mediasDaMediasTime.Tov += mediaTime.Tov;
                mediasDaMediasTime.Pt3 += mediaTime.Pt3;
            }

            mediasDaMediasTime.Pts = Math.Round(mediasDaMediasTime.Pts / 24, 3);
            mediasDaMediasTime.Reb = Math.Round(mediasDaMediasTime.Reb / 24, 3);
            mediasDaMediasTime.Ast = Math.Round(mediasDaMediasTime.Ast / 24, 3);
            mediasDaMediasTime.Blk = Math.Round(mediasDaMediasTime.Blk / 24, 3);
            mediasDaMediasTime.Stl = Math.Round(mediasDaMediasTime.Stl / 24, 3);
            mediasDaMediasTime.Tov = Math.Round(mediasDaMediasTime.Tov / 24, 3);
            mediasDaMediasTime.Pt3 = Math.Round(mediasDaMediasTime.Pt3 / 24, 3);


            return mediasDaMediasTime;
        }

        public IList<Jogador> jogadoresTime(int idTime)
        {
            string paramenters = "{\"class\":\"time\",\"action\":\"buscar\",\"params\":{\"timeId\":" + idTime + ",\"ligaId\":\"4\"}}";

            string response = getAPI(paramenters);

            JObject objTimes = JObject.Parse(response);
            JArray arrTimes = (JArray)objTimes["time"]["jogadores"];

            IList<Jogador> timeJogadores = arrTimes.ToObject<IList<Jogador>>();

            return timeJogadores;
        }

        public IList<Media> mediasJogador(int idJogador)
        {
            string paramenters = "{\"class\":\"jogador\",\"action\":\"detalhar\",\"params\":{\"jogadorId\":" + idJogador + ",\"ligaId\":\"4\"}}";

            string response = getAPI(paramenters);

            JObject objMediasJogador = JObject.Parse(response);
            JArray arrMedias = (JArray)objMediasJogador["medias"];

            IList<Media> mediasJogador = arrMedias.ToObject<IList<Media>>();

            return mediasJogador;
        }

        public IList<Media> mediasJogadorGenerico()
        {
            string paramenters = "{\"class\":\"estatisticas\",\"action\":\"buscarEstatisticasJogadores\",\"params\":{\":liga_id\":\"4\",\":temporada_id\":\"3\"}}";

            string response = getAPI(paramenters);

            JArray objMediasJogador = JArray.Parse(response);
            

            IList<Media> mediasJogador = objMediasJogador.ToObject<IList<Media>>();

            return mediasJogador;
        }

        public IList<Media> mediasJogadorLivre()
        {
            string paramenters = "{\"class\":\"jogador\",\"action\":\"listarFreeAgent\",\"params\":\"4\"}";

            string response = getAPI(paramenters);
            JArray objJogador = JArray.Parse(response);
            IList<Jogador> Jogadores = objJogador.ToObject<IList<Jogador>>();
            Media[] JogadoresMedia = new Media[Jogadores.Count()];

            int i = 0;

            foreach (Jogador jogador in Jogadores)
            {
                Console.Write(".");
                IList<Media> media = mediasJogador(jogador.Id);
                if (media.Count > 0 && media[0].Ano == 2019)
                {
                    JogadoresMedia[i] = mediasJogador(jogador.Id)[0];
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

                    JogadoresMedia[i] = jogadorZerado;
                }
                i++;
            }

            Console.WriteLine();

            return JogadoresMedia;
        }

        public IList<Media> mediasJogadorNBAAPI()
        {
            string response;
            
            string urlNBAAPI = @"https://stats.nba.com/stats/leagueLeaders";
            string parameters = "LeagueID=00&PerMode=PerGame&Scope=S&Season=2019-20&SeasonType=Regular+Season&StatCategory=PTS";



            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                response = wc.UploadString(urlNBAAPI, parameters);
            }
            
            JObject objMediasJogador = JObject.Parse(response);

            Media[] mediaJogadores = new Media[objMediasJogador["resultSet"]["rowSet"].Count()];

            

            int i = 0;
            foreach (var mediaJogadorNBA in objMediasJogador["resultSet"]["rowSet"])
            {
                mediaJogadores[i] = new Media();
                mediaJogadores[i].Nome = mediaJogadorNBA[2].ToString();
                mediaJogadores[i].Pts = Convert.ToDecimal(mediaJogadorNBA[22].ToString());
                mediaJogadores[i].Reb = Convert.ToDecimal(mediaJogadorNBA[17].ToString());
                mediaJogadores[i].Ast = Convert.ToDecimal(mediaJogadorNBA[18].ToString());
                mediaJogadores[i].Stl = Convert.ToDecimal(mediaJogadorNBA[19].ToString());
                mediaJogadores[i].Blk = Convert.ToDecimal(mediaJogadorNBA[20].ToString());
                mediaJogadores[i].Tov = Convert.ToDecimal(mediaJogadorNBA[21].ToString());
                mediaJogadores[i].Pt3 = Convert.ToDecimal(mediaJogadorNBA[9].ToString());

                i++;
            };                                                                         

            return mediaJogadores;
        }

        public Media allJogadoreMedia()
        {
            Media mediaTimes = this.allTimesMediaDasMedias();

            mediaTimes.Nome = "Media Jogadores";

            return mediaTimes;
        }

    }
}
