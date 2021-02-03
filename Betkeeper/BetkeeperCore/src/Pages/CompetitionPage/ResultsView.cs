using Betkeeper.Page.Components;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Betkeeper.Pages.CompetitionPage
{
    public partial class CompetitionPage
    {
        public Tab GetResultsTab(int competitionId)
        {
            var scores = TargetAction.CalculateCompetitionPoints(competitionId);

            var tableHeaderItems = new List<string>
            {
                "Bet",
                "Points",
                "Result"
            };

            tableHeaderItems.AddRange(scores.UserPointsDictionary.Keys.ToList());

            // Add header row (users) in constructor
            var table = new StaticTable(header: 
                new Row(
                    tableHeaderItems,
                    style: CellStyle.Bold));

            var summaryRowCellValues = new List<string> { "Total points", scores.MaximumPoints.ToString(), "-" };

            summaryRowCellValues.AddRange(scores.UserPointsDictionary.Values.Select(value => value.ToString()));

            // Add summary (users points)
            var summaryRow = new Row(
                summaryRowCellValues,
                CellStyle.Bold);
            table.Rows.Add(summaryRow);

            // Add data. Question first, then bets in same order as in header
            scores.TargetItems.ForEach(target =>
            {
                var row = new Row();

                row.Cells.Add(new Cell
                {
                    Style = CellStyle.Bold,
                    Value = target.Question
                });

                row.Cells.Add(new Cell
                {
                    Style = CellStyle.Bold,
                    Value = target.PointsAvailable
                });

                row.Cells.Add(new Cell
                {
                    Style = CellStyle.Bold,
                    Value = target.Result
                });

                target.BetItems.ForEach(bet =>
                {
                    Color backgroundColor;

                    switch (bet.Result)
                    {
                        case Enums.TargetResult.CorrectResult:
                            backgroundColor = Color.Green;
                            break;

                        case Enums.TargetResult.CorrectWinner:
                            backgroundColor = Color.LightGreen;
                            break;

                        case Enums.TargetResult.Wrong:
                            backgroundColor = Color.Red;
                            break;

                        default:
                            backgroundColor = Color.Gray;
                            break;
                    }
                    row.Cells.Add(new Cell
                    {
                        Color = backgroundColor,
                        Value = bet.Bet
                    });
                });

                table.Rows.Add(row);
            });

            return new Tab("resultsViewTab", "Results", new List<Component>
            {
                table
            });
        }
    }
}
