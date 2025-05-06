using System.Text.Json;
using System.ComponentModel;
using GameLibrary;

namespace GameBacklogManager
{
    public partial class MainForm : Form
    {
        private GameManager gameManager = new GameManager();
        private BindingList<Game> bindingList;
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private Game currentGame;
        private PlayingWindow playingWindow;

        private DataGridView dataGridView;
        private Button btnAdd, btnEdit, btnDelete, btnStart, btnFilter, btnSort, btnShowAll, btnFilterAdvanced, btnShortGames;
        private ComboBox cmbStatusFilter, cmbPlatformFilter;

        public MainForm()
        {
            Text = "Game Backlog Manager";
            Width = 1000;
            Height = 750;

            InitializeComponents();

            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;

            bindingList = new BindingList<Game>(gameManager.GetAllGames());
            dataGridView.DataSource = bindingList;

            ApplyStyle();
        }

        private void InitializeComponents()
        {
            var menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Plik");
            var saveItem = new ToolStripMenuItem("Zapisz", null, BtnSave_Click);
            var loadItem = new ToolStripMenuItem("Wczytaj", null, BtnLoad_Click);
            fileMenu.DropDownItems.Add(saveItem);
            fileMenu.DropDownItems.Add(loadItem);
            menuStrip.Items.Add(fileMenu);
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            menuStrip.BackColor = Color.WhiteSmoke;
            menuStrip.Font = new Font("Segoe UI", 10);
            menuStrip.RenderMode = ToolStripRenderMode.System;

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 450,
                AutoGenerateColumns = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            dataGridView.Columns.AddRange(
                new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Tytuł" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Genre", HeaderText = "Gatunek" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Platform", HeaderText = "Platforma" },
                new DataGridViewTextBoxColumn { DataPropertyName = "EstimatedPlaytimeMinutes", HeaderText = "Planowany czas" },
                new DataGridViewTextBoxColumn { DataPropertyName = "PlaytimeMinutes", HeaderText = "Czas grany" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Rating", HeaderText = "Ocena" },
                new DataGridViewTextBoxColumn { DataPropertyName = "ProgressPercent", HeaderText = "Postęp (%)" }
            );

            btnAdd = CreateStyledButton("Dodaj grę", BtnAdd_Click);
            btnEdit = CreateStyledButton("Edytuj grę", BtnEdit_Click);
            btnDelete = CreateStyledButton("Usuń grę", BtnDelete_Click);
            btnStart = CreateStyledButton("Symuluj granie", BtnStart_Click);
            btnFilter = CreateStyledButton("Filtruj 8+", BtnFilter_Click);
            btnSort = CreateStyledButton("Sortuj A-Z", BtnSort_Click);
            btnShowAll = CreateStyledButton("Pokaż wszystkie", (s, e) => ShowAllGames());
            btnShortGames = CreateStyledButton("Tylko krótkie gry", (s, e) =>
            {
                var filtered = gameManager.FilterGames(GameFilters.ShortGames);
                bindingList.Clear();
                foreach (var g in filtered)
                    bindingList.Add(g);
            });

            cmbStatusFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 150
            };
            cmbStatusFilter.Items.AddRange(Enum.GetNames(typeof(GameStatus)));
            cmbStatusFilter.SelectedIndex = -1;

            cmbPlatformFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 150
            };
            cmbPlatformFilter.Items.AddRange(new string[] { "PC", "PlayStation", "Xbox", "Switch" });
            cmbPlatformFilter.SelectedIndex = -1;

            btnFilterAdvanced = CreateStyledButton("Filtruj status/platformę", (s, e) =>
            {
                var filtered = gameManager.FilterGames(g =>
                    (cmbStatusFilter.SelectedIndex == -1 || g.Status.ToString() == cmbStatusFilter.SelectedItem.ToString()) &&
                    (cmbPlatformFilter.SelectedIndex == -1 || g.Platform == cmbPlatformFilter.SelectedItem.ToString()));

                bindingList.Clear();
                foreach (var g in filtered)
                    bindingList.Add(g);
            });

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 130,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(10)
            };

            panel.Controls.AddRange(new Control[]
            {
                btnAdd, btnEdit, btnDelete, btnStart,
                btnFilter, btnSort, btnShowAll,
                cmbStatusFilter, cmbPlatformFilter,
                btnFilterAdvanced, btnShortGames
            });

            Controls.Add(dataGridView);
            Controls.Add(panel);
        }

        private Button CreateStyledButton(string text, EventHandler clickEvent)
        {
            var btn = new Button
            {
                Text = text,
                AutoSize = true,
                Padding = new Padding(10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += clickEvent;
            return btn;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new AddGameForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Game game = form.GetGame();
                    gameManager.AddGame(game);
                    bindingList.Add(game);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd");
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (TryGetSelectedGame(out Game selected))
            {
                var form = new AddGameForm(selected);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    gameManager.UpdateGame(selected, form.GetGame());
                    bindingList.ResetBindings();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (TryGetSelectedGame(out Game selected))
            {
                gameManager.RemoveGame(selected);
                bindingList.Remove(selected);
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (TryGetSelectedGame(out Game selected))
            {
                if (selected.Status == GameStatus.Completed)
                {
                    MessageBox.Show("Gra już została ukończona.", "Informacja");
                    return;
                }

                currentGame = selected;
                currentGame.Status = GameStatus.InProgress;

                playingWindow = new PlayingWindow(currentGame.Title);
                playingWindow.PlayingWindowClosed += () =>
                {
                    gameTimer.Stop();
                    MessageBox.Show($"Zakończono symulację dla gry: {currentGame.Title}");
                };

                playingWindow.Show();
                gameTimer.Start();
                bindingList.ResetBindings();
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (currentGame != null)
            {
                currentGame.PlaytimeMinutes++;
                if (currentGame.PlaytimeMinutes >= currentGame.EstimatedPlaytimeMinutes)
                {
                    currentGame.Status = GameStatus.Completed;
                    gameTimer.Stop();
                    playingWindow?.Close();
                    MessageBox.Show($"{currentGame.Title} została ukończona!");
                }
                bindingList.ResetBindings();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var json = JsonSerializer.Serialize(bindingList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("games.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd");
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists("games.json")) return;

                var json = File.ReadAllText("games.json");
                var loaded = JsonSerializer.Deserialize<List<Game>>(json);

                gameManager = new GameManager();
                bindingList.Clear();
                foreach (var g in loaded)
                {
                    gameManager.AddGame(g);
                    bindingList.Add(g);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd odczytu: {ex.Message}", "Błąd");
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            var filtered = gameManager.FilterGames(g => g.Rating >= 8);
            bindingList.Clear();
            foreach (var g in filtered)
                bindingList.Add(g);
        }

        private void BtnSort_Click(object sender, EventArgs e)
        {
            var sorted = bindingList.OrderBy(g => g.Title).ToList();
            bindingList.Clear();
            foreach (var g in sorted)
                bindingList.Add(g);
        }

        private void ShowAllGames()
        {
            bindingList.Clear();
            foreach (var g in gameManager.GetAllGames())
                bindingList.Add(g);
        }

        private bool TryGetSelectedGame(out Game game)
        {
            game = dataGridView.SelectedRows.Count == 1 ? dataGridView.SelectedRows[0].DataBoundItem as Game : null;
            if (game == null)
            {
                MessageBox.Show("Zaznacz dokładnie jedną grę z listy.", "Błąd");
                return false;
            }
            return true;
        }

        private void ApplyStyle()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Font = new Font("Segoe UI", 10);

            foreach (Control ctrl in Controls)
            {
                if (ctrl is Panel || ctrl is FlowLayoutPanel)
                    ctrl.BackColor = Color.FromArgb(245, 245, 245);
            }

            dataGridView.DefaultCellStyle.BackColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dataGridView.BorderStyle = BorderStyle.FixedSingle;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.GridColor = Color.LightGray;
        }

        private class PlayingWindow : Form
        {
            private Label lblText, lblTime;
            private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer { Interval = 500 };
            private DateTime startTime = DateTime.Now;

            public event Action PlayingWindowClosed;

            public PlayingWindow(string gameTitle)
            {
                Text = $"Grasz w: {gameTitle}";
                Size = new Size(300, 150);
                StartPosition = FormStartPosition.CenterParent;
                BackColor = Color.White;
                Font = new Font("Segoe UI", 10);

                lblText = new Label { Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.SteelBlue };
                lblTime = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 14, FontStyle.Regular) };
                Controls.Add(lblTime);
                Controls.Add(lblText);

                animationTimer.Tick += (s, e) =>
                {
                    lblText.Text = $"Gramy{new string('.', DateTime.Now.Second % 4)}";
                    lblTime.Text = (DateTime.Now - startTime).ToString(@"hh\:mm\:ss");
                };
                animationTimer.Start();
            }

            protected override void OnFormClosed(FormClosedEventArgs e)
            {
                animationTimer.Stop();
                PlayingWindowClosed?.Invoke();
                base.OnFormClosed(e);
            }
        }
    }
}
