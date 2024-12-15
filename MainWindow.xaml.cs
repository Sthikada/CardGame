using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Lab3sem3;

namespace Lab4sem3
{
    public partial class MainWindow : Window
    {
        private Game game; // Основной объект игры.
        private Player currentPlayer; // Текущий игрок.
        private Player opponentPlayer; // Противник текущего игрока.
        private Card selectedCard; // Выбранная карта.

        public MainWindow()
        {
            InitializeComponent();

            // Загружаем сохраненное состояние игры при запуске программы
            game = Game.Load(); // Загрузка состояния или создание новой игры.
            currentPlayer = game.Player1; // Начинаем с игрока 1.
            opponentPlayer = game.Player2; // Противник — игрок 2.
            UpdateUI(); // Обновляем интерфейс.
        }

        private void UpdateUI()
        {
            // Обновляем информацию о существах.
            Player1CreatureName.Text = $"Имя: {game.Player1.Creature.Name}";
            Player1CreatureHealth.Text = $"Здоровье: {game.Player1.Creature.Health}";
            Player1CreatureAttack.Text = $"Атака: {game.Player1.Creature.Attack}";

            Player2CreatureName.Text = $"Имя: {game.Player2.Creature.Name}";
            Player2CreatureHealth.Text = $"Здоровье: {game.Player2.Creature.Health}";
            Player2CreatureAttack.Text = $"Атака: {game.Player2.Creature.Attack}";

            // Очищаем отображение карт обоих игроков.
            Player1Cards.Children.Clear();
            Player2Cards.Children.Clear();

            // Добавляем карты игрока 1.
            foreach (var card in game.Player1.Hand)
            {
                var cardButton = new Button
                {
                    Content = card.Name,
                    Tag = card,
                    Margin = new Thickness(5),
                    Width = 150,
                    Height = 50,
                    IsEnabled = currentPlayer == game.Player1 // Карты активны только для текущего игрока.
                };
                cardButton.Click += CardButton_Click;
                Player1Cards.Children.Add(cardButton);
            }

            // Добавляем карты игрока 2.
            foreach (var card in game.Player2.Hand)
            {
                var cardButton = new Button
                {
                    Content = card.Name,
                    Tag = card,
                    Margin = new Thickness(5),
                    Width = 150,
                    Height = 50,
                    IsEnabled = currentPlayer == game.Player2 // Карты активны только для текущего игрока.
                };
                cardButton.Click += CardButton_Click;
                Player2Cards.Children.Add(cardButton);
            }
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            // Обработка выбора карты.
            var button = sender as Button;
            if (button != null)
            {
                selectedCard = button.Tag as Card;
                GameLog.Text += $"Вы выбрали карту: {selectedCard.Name}\n";
            }
        }

        private void EndTurnButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCard != null)
            {
                // Применяем эффект карты перед завершением хода.
                selectedCard.Play();

                if (selectedCard is DamageSpell damageSpell)
                {
                    damageSpell.DealDamage(opponentPlayer.Creature);
                    GameLog.Text += $"{selectedCard.Name} нанес урон существу противника.\n";
                }
                else if (selectedCard is HealingSpell healingSpell)
                {
                    healingSpell.Heal(currentPlayer.Creature);
                    GameLog.Text += $"{selectedCard.Name} восстановил здоровье вашему существу.\n";
                }
                else if (selectedCard is BuffSpell buffSpell)
                {
                    buffSpell.Buff(currentPlayer.Creature);
                    GameLog.Text += $"{selectedCard.Name} усилил ваше существо.\n";
                }

                // Удаляем использованную карту.
                currentPlayer.Hand.Remove(selectedCard);
                selectedCard = null;
            }

            // Проверка окончания игры.
            if (!game.Player1.Hand.Any() && !game.Player2.Hand.Any())
            {
                EndGame();
                return;
            }

            // Смена хода.
            SwapTurn();
            UpdateUI();
        }

        private void SwapTurn()
        {
            // Меняем текущего игрока.
            if (currentPlayer == game.Player1)
            {
                currentPlayer = game.Player2;
                opponentPlayer = game.Player1;
                GameLog.Text += "Ход Игрока 2.\n";
            }
            else
            {
                currentPlayer = game.Player1;
                opponentPlayer = game.Player2;
                GameLog.Text += "Ход Игрока 1.\n";
            }
        }

        private void EndGame()
        {
            // Определяем победителя.
            string result = game.Player1.Creature.Health > game.Player2.Creature.Health
                ? "Игрок 1 побеждает!"
                : game.Player1.Creature.Health < game.Player2.Creature.Health
                    ? "Игрок 2 побеждает!"
                    : "Ничья!";

            MessageBox.Show(result, "Игра завершена");
            game = new Game(); // Создаём новую игру для перезапуска.
            currentPlayer = game.Player1;
            opponentPlayer = game.Player2;
            UpdateUI();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Сохраняем состояние игры при закрытии программы.
            Game.Save(game);
            base.OnClosing(e);
        }
    }
}
