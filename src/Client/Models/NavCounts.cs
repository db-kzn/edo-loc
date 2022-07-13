using System;

namespace EDO_FOMS.Client.Models
{
    public class NavCounts
    {
        private int progress;
        private int docs;
        private int archive;

        public int Progress
        {
            get => this.progress;
            set
            {
                if (value != this.progress)
                {
                    this.progress = value;
                    CountChanged?.Invoke();
                }
            }
        }
        
        public int Docs
        {
            get => this.docs;
            set
            {
                if (value != this.docs)
                {
                    this.docs = value;
                    CountChanged?.Invoke();
                }
            }
        }

        public int Archive
        {
            get => this.archive;
            set
            {
                if (value != this.archive)
                {
                    this.archive = value;
                    CountChanged?.Invoke();
                }
            }
        }

        public Action CountChanged { get; set; }
    }
}
