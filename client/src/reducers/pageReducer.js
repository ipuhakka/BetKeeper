import _ from 'lodash';
import * as pageActions from '../actions/pageActions';

function handleGetPageSuccess(state, newPage)
{
    const pages = _.cloneDeep(state.pages);

    const pageIndexToReplace = _.findIndex(pages, page => page.key === newPage.key);

    if (pageIndexToReplace === -1)
    {
        pages.push(newPage);
    }
    else 
    {
        pages[pageIndexToReplace] = newPage;
    }

    return pages;
}

/**
 * PageReducer.
 * @param {object} state
 * @param {object} action 
 */
const PageReducer = (state = { pages: []}, action ) => {
    switch (action.type) 
    {
      case pageActions.GET_PAGE_SUCCESS:
        const pages = handleGetPageSuccess();
        return {
            ...state,
            pages: pages
        };

      default:
        return state;
    }
  };

  export default PageReducer;