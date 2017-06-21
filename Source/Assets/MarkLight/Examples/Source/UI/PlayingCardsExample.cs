using MarkLight.Examples.Data;

namespace MarkLight.Examples.UI
{
    /// <summary>
    /// Example showing dynamic list of cards in a scrollable wrapped list featuring multiple templates.
    /// </summary>
    public class PlayingCardsExample : View
    {
        #region Fields

        public ObservableList<Card> Cards;
        private readonly System.Random _random = new System.Random();

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Cards = new ObservableList<Card>();

            // generate random cards            
            for (var i = 1; i <= 22; ++i)
            {
                Add();
            }
        }

        /// <summary>
        /// Adds new card to the list.
        /// </summary>
        public void Add()
        {
            var card = new Card
            {
                CardRank = _random.Next(11, 15),
                CardSuit = (CardSuit) _random.Next(1, 4)
            };
            Cards.Add(card);
        }

        /// <summary>
        /// Removes selected card from the list.
        /// </summary>
        public void Remove()
        {
            Cards.RemoveAt(Cards.SelectedIndex);
        }

        /// <summary>
        /// Scroll to card in list.
        /// </summary>
        public void ScrollTo()
        {
            Cards.ScrollTo(Cards.SelectedItem);
        }
        
        #endregion
    }
}

