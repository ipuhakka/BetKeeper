import _ from 'lodash';
import * as pageActions from '../actions/pageActions';
import * as pageUtils from '../js/pageUtils';

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

function updateOptions(state, response)
{
  const { pages } = state;
  
  const pageKey = window.location.pathname.replace('/page/', '');

  const page = _.find(pages, page => page.key === pageKey);

  if (_.isNil(page))
  {
    return;
  }
  
  // Set component options list for dropdowns which keys are found from response 
  Object.keys(response).forEach(key => 
    {
      const component = pageUtils.findComponentFromPage(page, key);

      if (_.isNil(component) || _.get(component, 'fieldType', null) !== 'Dropdown')
      {
        return;
      }

      component.options = response[key];
    });

    return state;
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
      pageUtils.replaceComponent(page, component);
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
      case pageActions.GET_PAGE_SUCCESS:
        const newPages = handleGetPageSuccess(state, action.payload.structure);

        return {
            ...state,
            pages: newPages
        };

      case pageActions.UPDATE_OPTIONS_SUCCESS:
        return updateOptions(_.cloneDeep(state), action.payload.response);

      case pageActions.UPDATE_COMPONENTS:
        const { page, components } = action.payload;
        return handleUpdateComponents(_.cloneDeep(state), page, components)

      default:
        return state;
    }
  };

  export default PageReducer;