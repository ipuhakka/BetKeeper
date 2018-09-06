using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BetKeeper
{
    public class Bet
    {
        [JsonProperty]
        private int owner;
        [JsonProperty]
        private string name;
        [JsonProperty]
        private string datetime;
        [JsonProperty]
        private bool? bet_won; //null = unresolved, false = lost, true = won.
        [JsonProperty]
        private int bet_id;
        [JsonProperty]
        private double odd;
        [JsonProperty]
        private double bet;

        public Bet(int user_id, string bet_name, string date, int res, int id, double odd_par, double bet_par)
        {
            owner = user_id;
            name = bet_name;
            datetime = date;
            bet_id = id;
            odd = odd_par;
            bet = bet_par;

            if (res == -1)
                bet_won = null;
            else
                bet_won = Convert.ToBoolean(res);
        }

        public int getId()
        {
            return bet_id;
        }

        public int getOwner() { return owner; }
    }
}
