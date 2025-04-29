using GameLibrary;

namespace GameBacklogManager
{
    public class AddGameForm : Form
    {
        private TextBox txtTitle, txtGenre, txtPlatform, txtPlaytime, txtRating;
        private ComboBox cmbStatus;
        private Button btnSave;

        private Game createdGame;
        private Game editingGame = null;

        public AddGameForm()
        {
            this.Text = "Dodaj / Edytuj grę";
            this.Width = 350;
            this.Height = 400;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Font = new Font("Segoe UI", 10);

            InitializeComponents();
        }

        public AddGameForm(Game gameToEdit) : this()
        {
            editingGame = gameToEdit;

            txtTitle.Text = gameToEdit.Title;
            txtGenre.Text = gameToEdit.Genre;
            txtPlatform.Text = gameToEdit.Platform;
            txtPlaytime.Text = gameToEdit.EstimatedPlaytimeMinutes.ToString();
            txtRating.Text = gameToEdit.Rating.ToString();
            cmbStatus.SelectedItem = gameToEdit.Status.ToString();
        }

        private void InitializeComponents()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 7,
                ColumnCount = 2,
                Padding = new Padding(10),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

            layout.Controls.Add(new Label { Text = "Tytuł gry:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            txtTitle = new TextBox();
            layout.Controls.Add(txtTitle, 1, 0);

            layout.Controls.Add(new Label { Text = "Gatunek:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            txtGenre = new TextBox();
            layout.Controls.Add(txtGenre, 1, 1);

            layout.Controls.Add(new Label { Text = "Platforma:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            txtPlatform = new TextBox();
            layout.Controls.Add(txtPlatform, 1, 2);

            layout.Controls.Add(new Label { Text = "Czas [min]:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            txtPlaytime = new TextBox();
            layout.Controls.Add(txtPlaytime, 1, 3);

            layout.Controls.Add(new Label { Text = "Ocena [1–10]:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight }, 0, 4);
            txtRating = new TextBox();
            layout.Controls.Add(txtRating, 1, 4);

            layout.Controls.Add(new Label { Text = "Status:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight }, 0, 5);
            cmbStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(Enum.GetNames(typeof(GameStatus)));
            cmbStatus.SelectedIndex = 0;
            layout.Controls.Add(cmbStatus, 1, 5);

            btnSave = new Button
            {
                Text = "Zapisz",
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Height = 40
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            layout.Controls.Add(btnSave, 0, 6);
            layout.SetColumnSpan(btnSave, 2);

            this.Controls.Add(layout);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string title = txtTitle.Text.Trim();
                string genre = txtGenre.Text.Trim();
                string platform = txtPlatform.Text.Trim();

                if (!int.TryParse(txtPlaytime.Text.Trim(), out int playtime) || playtime < 0)
                    throw new ArgumentException("Czas gry musi być dodatnią liczbą całkowitą.");

                if (!int.TryParse(txtRating.Text.Trim(), out int rating) || rating < 1 || rating > 10)
                    throw new ArgumentException("Ocena musi być w zakresie 1–10.");

                if (string.IsNullOrWhiteSpace(title))
                    throw new InvalidGameTitleException(title);

                if (string.IsNullOrWhiteSpace(genre))
                    throw new ArgumentException("Gatunek gry nie może być pusty.");

                if (string.IsNullOrWhiteSpace(platform))
                    throw new ArgumentException("Platforma gry nie może być pusta.");

                createdGame = new Game
                {
                    Title = title,
                    Genre = genre,
                    Platform = platform,
                    EstimatedPlaytimeMinutes = playtime,
                    Rating = rating,
                    Status = (GameStatus)Enum.Parse(typeof(GameStatus), cmbStatus.SelectedItem.ToString()),
                    PlaytimeMinutes = editingGame?.PlaytimeMinutes ?? 0
                };

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        public Game GetGame() => createdGame;
    }
}
