import _ from 'lodash';
import * as PageActions from '../actions/pageActions';
import * as PageUtils from '../js/pageUtils';

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
 * Updates page components.
 * @param {object} state 
 * @param {string} pageKey
 * @param {object} actionResponse 
 */
function handleUpdateComponents(state, pageKey, components)
{
  const page = _.find(state.pages, page => page.key === pageKey);

  if (_.isNil(page))
  {
    return state;
  }

  _.forEach(components, component => 
    {
      PageUtils.replaceComponent(page, component);
    });

  return state;
}

/**
 * PageReducer.
 * @param {object} state
 * @param {object} action 
 */
const PageReducer = (state = { pages: []}, action ) => {
    switch (action.type) 
    {
      case PageActions.GET_PAGE_SUCCESS:
        const newPages = handleGetPageSuccess(state, action.payload.structure);

        return {
            ...state,
            pages: newPages
        };

      case PageActions.UPDATE_COMPONENTS:
        const { page, components } = action.payload;
        return handleUpdateComponents(_.cloneDeep(state), page, components)

      default:
        return state;
    }
  };

  export default PageReducer;