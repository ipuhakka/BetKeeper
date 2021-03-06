﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Betkeeper.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Betkeeper.Models
{
    public class TargetBet
    {
        [Key]
        public int TargetBetId { get; set; }

        public int Target { get; set; }

        public int? Participator { get; set; }

        public string Bet { get; set; }

        /// <summary>
        /// Creates target bet from JObject.
        /// JObject-format: 
        /// {
        ///     target-{targetId}: 
        ///     {
        ///         bet-answer-{targetId}: "answer"
        ///         // or bet-selection-{targetId} with selection type
        ///     }
        /// }
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TargetBet FromJObject(JObject jObject)
        {
            var targetId = int.Parse(jObject
                .Properties()
                .Single(property => property.Name.Contains("target-"))
                .Name
                .ToString()
                .Split('-')
                .Last());

            var value = jObject[$"target-{targetId}"][$"bet-answer-{targetId}"];

            string bet;
            if (value is JArray)
            {
                bet = JsonConvert.SerializeObject(value.ToObject<List<string>>());
            }
            else
            {
                bet = value.ToString();
            }

            return new TargetBet
            {
                Target = targetId,
                Bet = bet
            };
        }
    }

    public class TargetBetRepository
    {
        private readonly BetkeeperDataContext _context;

        public TargetBetRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        /// <summary>
        /// Adds new target bets.
        /// </summary>
        /// <param name="targetBets"></param>
        public void AddTargetBets(List<TargetBet> targetBets)
        {
            if (targetBets.Any(targetBet => targetBet.Participator == null))
            {
                throw new InvalidOperationException("No participator set");
            }

            _context.TargetBet.AddRange(targetBets);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates target bets.
        /// </summary>
        /// <param name="targetBets"></param>
        public void UpdateTargetBets(List<TargetBet> targetBets)
        {
            if (targetBets.Any(targetBet => targetBet.Participator == null))
            {
                throw new InvalidOperationException("No participator set");
            }

            _context.TargetBet.UpdateRange(targetBets);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get target bets from a competition.
        /// </summary>
        /// <param name="participator"></param>
        /// <param name="targetId"></param>
        public List<TargetBet> GetTargetBets(
            int? participator = null, 
            List<int> targetIds = null)
        {
            var query = _context.TargetBet.AsQueryable();

            if (participator != null)
            {
                query = query.Where(targetBet => 
                    targetBet.Participator == participator);
            }

            if (targetIds != null)
            {
                query = query.Where(targetBet =>
                    targetIds.Contains(targetBet.Target));
            }

            return query.ToList();
        }
    }
}
