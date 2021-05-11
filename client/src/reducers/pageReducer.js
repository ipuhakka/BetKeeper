import _ from 'lodash';
import * as PageActions from '../actions/pageActions';
import * as PageUtils from '../js/pageUtils';

function handleGetPageSuccess(state, newPage)
{
  const pages = _.cloneDeep(state.pages);

  const pageIndexToReplace = _.findIndex(pages, page => page.pageKey === newPage.pageKey);

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
  const page = _.find(state.pages, page => page.pageKey === pageKey);

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
 * Updates page data
 * @param {object} state 
 * @param {string} pageKey 
 * @param {*} data 
 */
function handleUpdateData(state, pageKey, data)
{
  const page = _.find(state.pages, page => page.pageKey === pageKey);

  if (_.isNil(page))
  {
    return state;
  }

  _.forEach(Object.keys(data), dataKey => 
  {
    PageUtils.replaceData(page, dataKey, data[dataKey]);
  });

  return state;
}

/**
 * Handles page data change.
 * @param {object} state 
 * @param {object} action 
 */
function handleDataChange(state, action)
{
  const { pageKey, dataKeyPath, newValue } = action.payload;

  const page = _.find(state.pages, page =>
    page.pageKey === pageKey);

  _.set(page.data, dataKeyPath, newValue);
    
  return state;
}

/**
 * Update dropdown selections for page.
 * Used for filling an input dropdown from an already created list
 * @param {*} state
 * @param {string} pageKey 
 * @param {string} componentKey 
 * @param {Array} selections 
 */
function handleDropdownSelectionUpdate(state, pageKey, componentKey, selections)
{
  const page = _.find(state.pages, page =>
    page.pageKey === pageKey);

  _.set(page, `existingSelections.${componentKey}`, selections);

  return state;
}

/**
 * PageReducer.
 * @param {object} state
 * @param {object} action 
 */
const PageReducer = (state = { pages: []}, action ) => 
{
    switch (action.type) 
    {
      case PageActions.GET_PAGE_SUCCESS:
        const newPages = handleGetPageSuccess(state, action.payload.structure);

        return {
            ...state,
            pages: newPages
        };

      case PageActions.UPDATE_COMPONENTS: 
      {
        const { page, components } = action.payload;
        return handleUpdateComponents(_.cloneDeep(state), page, components);
      }

      case PageActions.DATA_CHANGE:
        return handleDataChange(_.cloneDeep(state), action);

      case PageActions.UPDATE_DATA:
        {
          const { page, data } = action.payload;
          return handleUpdateData(_.cloneDeep(state), page, data);
        }

      case PageActions.UPDATE_DROPDOWN_SELECTIONS:
        {
          const { pageKey, componentKey, selections } = action.payload;
          return handleDropdownSelectionUpdate(_.cloneDeep(state), pageKey, componentKey, selections);
        }
        
      default:
        return state;
    }
  };

  export default PageReducer;