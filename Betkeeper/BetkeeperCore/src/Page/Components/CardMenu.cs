using System.Collections.Generic;

namespace Betkeeper.Page.Components
{
    public class CardMenu : Component
    {
        public List<Card> Cards { get; set; }

        public CardMenu(List<Card> cards, string componentKey)
            :base(ComponentType.CardMenu, componentKey)
        {
            Cards = cards;
        }
    }

    public class Card
    {
        public string Image { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string NavigateTo { get; set; }

        public Card(string image, string title, string text, string navigateTo)
        {
            Image = image;
            Title = title;
            Text = text;
            NavigateTo = navigateTo;
        }
    }
}
