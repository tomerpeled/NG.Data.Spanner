using System.Data;
using System.Data.Common;

namespace NG.Data.Spanner
{
    public class SpannerDataAdapter: DbDataAdapter, IDbDataAdapter
    {

        private SpannerCommand _selectCommand;
        private SpannerCommand _insertCommand;
        private SpannerCommand _deleteCommand;
        private SpannerCommand _updateCommand;


        #region Properties
        public new SpannerCommand SelectCommand
        {
            get => _selectCommand;
	        set => _selectCommand = value;
        }

        IDbCommand IDbDataAdapter.SelectCommand
        {
            get => _selectCommand;
	        set => _selectCommand = (SpannerCommand)value;
        }

        public new SpannerCommand InsertCommand
        {
            get => _insertCommand;
	        set => _insertCommand = value;
        }

        IDbCommand IDbDataAdapter.InsertCommand
        {
            get => _insertCommand;
	        set => _insertCommand = (SpannerCommand)value;
        }

        public new SpannerCommand UpdateCommand
        {
            get => _updateCommand;
	        set => _updateCommand = value;
        }

        IDbCommand IDbDataAdapter.UpdateCommand
        {
            get => _updateCommand;
	        set => _updateCommand = (SpannerCommand)value;
        }

        public new SpannerCommand DeleteCommand
        {
            get => _deleteCommand;
	        set => _deleteCommand = value;
        }

        IDbCommand IDbDataAdapter.DeleteCommand
        {
            get => _deleteCommand;
	        set => _deleteCommand = (SpannerCommand)value;
        }
        #endregion

    }
}
