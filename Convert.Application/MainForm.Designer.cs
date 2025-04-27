namespace Convert.Application
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._menuImport = new System.Windows.Forms.ToolStripMenuItem();
            this._menuExport = new System.Windows.Forms.ToolStripMenuItem();
            this._itemEachToSingle = new System.Windows.Forms.ToolStripMenuItem();
            this._itemEachToMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._itemMergeCsvRows = new System.Windows.Forms.ToolStripMenuItem();
            this._itemMergeCsvColums = new System.Windows.Forms.ToolStripMenuItem();
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this._menuEncoding = new System.Windows.Forms.ToolStripMenuItem();
            this._itemReadEncoding = new System.Windows.Forms.ToolStripMenuItem();
            this._itemWriteEncoding = new System.Windows.Forms.ToolStripMenuItem();
            this._menuRegister = new System.Windows.Forms.ToolStripMenuItem();
            this._itemRegister = new System.Windows.Forms.ToolStripMenuItem();
            this._itemUnregister = new System.Windows.Forms.ToolStripMenuItem();
            this._tableFiles = new System.Windows.Forms.DataGridView();
            this.colFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
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
            this._itemEachToMulti,
            this.toolStripSeparator1,
            this._itemMergeCsvRows,
            this._itemMergeCsvColums});
            this._menuExport.Name = "_menuExport";
            this._menuExport.Size = new System.Drawing.Size(44, 21);
            this._menuExport.Text = "导出";
            // 
            // _itemEachToSingle
            // 
            this._itemEachToSingle.Enabled = false;
            this._itemEachToSingle.Name = "_itemEachToSingle";
            this._itemEachToSingle.Size = new System.Drawing.Size(202, 22);
            this._itemEachToSingle.Text = "每个文件导出为一个csv";
            this._itemEachToSingle.Click += new System.EventHandler(this.ItemExport_Click);
            // 
            // _itemEachToMulti
            // 
            this._itemEachToMulti.Enabled = false;
            this._itemEachToMulti.Name = "_itemEachToMulti";
            this._itemEachToMulti.Size = new System.Drawing.Size(202, 22);
            this._itemEachToMulti.Text = "每个文件分解为多个csv";
            this._itemEachToMulti.Click += new System.EventHandler(this.ItemExport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
            // 
            // _itemMergeCsvRows
            // 
            this._itemMergeCsvRows.Enabled = false;
            this._itemMergeCsvRows.Name = "_itemMergeCsvRows";
            this._itemMergeCsvRows.Size = new System.Drawing.Size(202, 22);
            this._itemMergeCsvRows.Text = "按行合并csv";
            this._itemMergeCsvRows.Click += new System.EventHandler(this.ItemExportMergedCsv_Click);
            // 
            // _itemMergeCsvColums
            // 
            this._itemMergeCsvColums.Enabled = false;
            this._itemMergeCsvColums.Name = "_itemMergeCsvColums";
            this._itemMergeCsvColums.Size = new System.Drawing.Size(202, 22);
            this._itemMergeCsvColums.Text = "按列合并csv";
            this._itemMergeCsvColums.Click += new System.EventHandler(this.ItemExportMergedCsv_Click);
            // 
            // _menuStrip
            // 
            this._menuStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._menuImport,
            this._menuExport,
            this._menuEncoding,
            this._menuRegister});
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this._menuStrip.Size = new System.Drawing.Size(471, 25);
            this._menuStrip.TabIndex = 0;
            this._menuStrip.Text = "menuStrip1";
            // 
            // _menuEncoding
            // 
            this._menuEncoding.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._menuEncoding.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._itemReadEncoding,
            this._itemWriteEncoding});
            this._menuEncoding.Name = "_menuEncoding";
            this._menuEncoding.Size = new System.Drawing.Size(68, 21);
            this._menuEncoding.Text = "编码格式";
            // 
            // _itemReadEncoding
            // 
            this._itemReadEncoding.BackColor = System.Drawing.SystemColors.Control;
            this._itemReadEncoding.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._itemReadEncoding.Name = "_itemReadEncoding";
            this._itemReadEncoding.Size = new System.Drawing.Size(180, 22);
            this._itemReadEncoding.Text = "读取";
            // 
            // _itemWriteEncoding
            // 
            this._itemWriteEncoding.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._itemWriteEncoding.Name = "_itemWriteEncoding";
            this._itemWriteEncoding.Size = new System.Drawing.Size(180, 22);
            this._itemWriteEncoding.Text = "写入";
            // 
            // _menuRegister
            // 
            this._menuRegister.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._menuRegister.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._menuRegister.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._itemRegister,
            this._itemUnregister});
            this._menuRegister.Name = "_menuRegister";
            this._menuRegister.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._menuRegister.Size = new System.Drawing.Size(44, 21);
            this._menuRegister.Text = "注册";
            // 
            // _itemRegister
            // 
            this._itemRegister.Name = "_itemRegister";
            this._itemRegister.Size = new System.Drawing.Size(160, 22);
            this._itemRegister.Text = "注册到右键菜单";
            // 
            // _itemUnregister
            // 
            this._itemUnregister.Name = "_itemUnregister";
            this._itemUnregister.Size = new System.Drawing.Size(160, 22);
            this._itemUnregister.Text = "从右键菜单移除";
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
            this._tableFiles.ReadOnly = true;
            this._tableFiles.RowTemplate.Height = 23;
            this._tableFiles.Size = new System.Drawing.Size(471, 282);
            this._tableFiles.TabIndex = 1;
            this._tableFiles.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.TableFiles_RowPostPaint);
            this._tableFiles.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.RowsAdded);
            this._tableFiles.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.RowsRemoved);
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
            this.colFileName.ReadOnly = true;
            // 
            // colProgress
            // 
            this.colProgress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colProgress.FillWeight = 60.9137F;
            this.colProgress.HeaderText = "进度   ";
            this.colProgress.MinimumWidth = 70;
            this.colProgress.Name = "colProgress";
            this.colProgress.ReadOnly = true;
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
            this.Text = "SyncConvert";
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
        private System.Windows.Forms.ToolStripMenuItem _itemRegister;
        private System.Windows.Forms.ToolStripMenuItem _itemUnregister;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProgress;
        private System.Windows.Forms.ToolTip _toolTip;
        private System.Windows.Forms.ToolStripMenuItem _menuEncoding;
        private System.Windows.Forms.ToolStripMenuItem _itemReadEncoding;
        private System.Windows.Forms.ToolStripMenuItem _itemWriteEncoding;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _itemMergeCsvRows;
        private System.Windows.Forms.ToolStripMenuItem _itemMergeCsvColums;
    }
}

