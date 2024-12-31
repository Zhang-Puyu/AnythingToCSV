namespace Processor.Application
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._menuImport = new System.Windows.Forms.ToolStripMenuItem();
            this._menuExport = new System.Windows.Forms.ToolStripMenuItem();
            this._itemEachToSingle = new System.Windows.Forms.ToolStripMenuItem();
            this._itemEachToMulti = new System.Windows.Forms.ToolStripMenuItem();
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this._menuRegister = new System.Windows.Forms.ToolStripMenuItem();
            this._itemAddToMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._itemRemoveFromMenu = new System.Windows.Forms.ToolStripMenuItem();
            this._tableFiles = new System.Windows.Forms.DataGridView();
            this.colFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tableFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // _menuImport
            // 
            this._menuImport.Name = "_menuImport";
            this._menuImport.Size = new System.Drawing.Size(44, 21);
            this._menuImport.Text = "打开";
            // 
            // _menuExport
            // 
            this._menuExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._itemEachToSingle,
            this._itemEachToMulti});
            this._menuExport.Name = "_menuExport";
            this._menuExport.Size = new System.Drawing.Size(44, 21);
            this._menuExport.Text = "导出";
            // 
            // _itemEachToSingle
            // 
            this._itemEachToSingle.Name = "_itemEachToSingle";
            this._itemEachToSingle.Size = new System.Drawing.Size(207, 22);
            this._itemEachToSingle.Text = "每个文件导出为一个CSV";
            this._itemEachToSingle.Click += new System.EventHandler(this.ItemExport_Click);
            // 
            // _itemEachToMulti
            // 
            this._itemEachToMulti.Name = "_itemEachToMulti";
            this._itemEachToMulti.Size = new System.Drawing.Size(207, 22);
            this._itemEachToMulti.Text = "每个文件分解为多个CSV";
            this._itemEachToMulti.Click += new System.EventHandler(this.ItemExport_Click);
            // 
            // _menuStrip
            // 
            this._menuStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuImport,
            this._menuExport,
            this._menuRegister});
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this._menuStrip.Size = new System.Drawing.Size(471, 25);
            this._menuStrip.TabIndex = 0;
            this._menuStrip.Text = "menuStrip1";
            // 
            // _menuRegister
            // 
            this._menuRegister.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._itemAddToMenu,
            this._itemRemoveFromMenu});
            this._menuRegister.Name = "_menuRegister";
            this._menuRegister.Size = new System.Drawing.Size(44, 21);
            this._menuRegister.Text = "注册";
            // 
            // _itemAddToMenu
            // 
            this._itemAddToMenu.Name = "_itemAddToMenu";
            this._itemAddToMenu.Size = new System.Drawing.Size(184, 22);
            this._itemAddToMenu.Text = "添加到文件右键菜单";
            this._itemAddToMenu.Click += new System.EventHandler(this.ItemAddToContextMenu_Click);
            // 
            // _itemRemoveFromMenu
            // 
            this._itemRemoveFromMenu.Name = "_itemRemoveFromMenu";
            this._itemRemoveFromMenu.Size = new System.Drawing.Size(184, 22);
            this._itemRemoveFromMenu.Text = "从文件右键菜单移除";
            this._itemRemoveFromMenu.Click += new System.EventHandler(this.ItemRemoveFromContextMenu_Click);
            // 
            // _tableFiles
            // 
            this._tableFiles.AllowDrop = true;
            this._tableFiles.AllowUserToAddRows = false;
            this._tableFiles.AllowUserToDeleteRows = false;
            this._tableFiles.AllowUserToResizeRows = false;
            this._tableFiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._tableFiles.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this._tableFiles.BackgroundColor = System.Drawing.SystemColors.Control;
            this._tableFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._tableFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFileName,
            this.colProgress});
            this._tableFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableFiles.Location = new System.Drawing.Point(0, 25);
            this._tableFiles.Margin = new System.Windows.Forms.Padding(5);
            this._tableFiles.Name = "_tableFiles";
            this._tableFiles.RowTemplate.Height = 23;
            this._tableFiles.Size = new System.Drawing.Size(471, 282);
            this._tableFiles.TabIndex = 1;
            this._tableFiles.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.TableFiles_RowPostPaint);
            this._tableFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this._tableFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            // 
            // colFileName
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colFileName.DefaultCellStyle = dataGridViewCellStyle1;
            this.colFileName.FillWeight = 89.08628F;
            this.colFileName.HeaderText = "文件名";
            this.colFileName.Name = "colFileName";
            // 
            // colProgress
            // 
            this.colProgress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colProgress.FillWeight = 60.9137F;
            this.colProgress.HeaderText = "进度   ";
            this.colProgress.MinimumWidth = 70;
            this.colProgress.Name = "colProgress";
            this.colProgress.Width = 70;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 307);
            this.Controls.Add(this._tableFiles);
            this.Controls.Add(this._menuStrip);
            this.MainMenuStrip = this._menuStrip;
            this.Name = "MainForm";
            this.Text = "SyncProcess";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tableFiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem _menuImport;
        private System.Windows.Forms.ToolStripMenuItem _menuExport;
        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem _itemEachToMulti;
        private System.Windows.Forms.ToolStripMenuItem _itemEachToSingle;
        private System.Windows.Forms.DataGridView _tableFiles;
        private System.Windows.Forms.ToolStripMenuItem _menuRegister;
        private System.Windows.Forms.ToolStripMenuItem _itemAddToMenu;
        private System.Windows.Forms.ToolStripMenuItem _itemRemoveFromMenu;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProgress;
    }
}

