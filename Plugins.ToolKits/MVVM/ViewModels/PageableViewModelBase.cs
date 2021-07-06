using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Plugins.ToolKits.MVVM
{
    public abstract class PageableViewModelBase : ViewModelBase
    {
        private int _currentPage = 1;
        private bool _isSearching;
        private string _oldSearchWord = string.Empty;
        private int _pageSize = 10;
        private string _searchKey = "";
        private int _targetPage = 1;
        private int _totalPage = 1;

        public int TotalPage
        {
            get => _totalPage;
            set => SetProperty(ref _totalPage, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public int TargetPage
        {
            get => _targetPage;
            set => SetProperty(ref _targetPage, value);
        }

        public virtual int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public string SearchKeyword
        {
            get => _searchKey;
            set => SetProperty(ref _searchKey, value);
        }

        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }
        public ICommand SearchCommand => CommandBinder.BindExclusiveCommand(async (exclusiveContext) =>
    {
        exclusiveContext.BeginExclusive();
        try
        {
            if (IsSearching)
            {
                return;
            }

            IsSearching = true;
            string search = SearchKeyword;
            if (_oldSearchWord != search)
            {
                CurrentPage = 1;
            }

            int totalCount = await Search(search, CurrentPage, PageSize);

            TotalPage = (int)Math.Ceiling((double)totalCount / PageSize);

            _oldSearchWord = search;
        }
        finally
        {
            IsSearching = false;
            exclusiveContext.EndExclusive();
        }
    });


        public ICommand GotoCommand => CommandBinder.BindExclusiveCommand((context) =>
      {
          try
          {
              context.BeginExclusive();
              if (TargetPage > TotalPage || CurrentPage == TargetPage)
              {
                  return;
              }

              if (TargetPage < 1 || CurrentPage == 1)
              {
                  return;
              }

              CurrentPage = TargetPage;

              Search(SearchKeyword, CurrentPage, PageSize);
          }
          finally
          {
              context.EndExclusive();
          }


      });

        public ICommand FirstPageCommand => (CommandBinder.BindExclusiveCommand((context) =>
       {
           try
           {
               context.BeginExclusive();
               CurrentPage = 1;
               SearchCommand?.Execute(null);
           }
           finally
           {
               context.EndExclusive();
           }

       }));

        public ICommand PreviousPageCommand => CommandBinder.BindExclusiveCommand((context) =>
      {
          try
          {
              context.BeginExclusive();

              if (CurrentPage == 1)
              {
                  return;
              }

              CurrentPage -= 1;
              SearchCommand?.Execute(null);
          }
          finally
          {
              context.EndExclusive();
          }

      });

        public ICommand LastPageCommand => (CommandBinder.BindExclusiveCommand((context) =>
      {
          try
          {
              context.BeginExclusive();

              CurrentPage = TotalPage;
              SearchCommand?.Execute(null);
          }
          finally
          {
              context.EndExclusive();
          }

      }));

        public ICommand NextPageCommand => (CommandBinder.BindExclusiveCommand((context) =>
       {

           try
           {
               context.BeginExclusive();

               if (CurrentPage == TotalPage)
               {
                   return;
               }

               CurrentPage += 1;
               SearchCommand?.Execute(null);
           }
           finally
           {
               context.EndExclusive();
           }

       }));


        /// <summary>
        ///     <para>keyword:keyword of search</para>
        ///     <para>currentPage:the number of page</para>
        ///     <para>pageSize:max count in a page</para>
        ///     <para>returns:the number of total count</para>
        /// </summary>
        /// <param name="keyword">keyword of search</param>
        /// <param name="currentPage">the number of page</param>
        /// <param name="pageSize">the count in a page</param>
        /// <returns>total count</returns>
        protected abstract Task<int> Search(string keyword, int currentPage, int pageSize);
    }
}