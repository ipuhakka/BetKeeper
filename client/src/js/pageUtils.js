import _ from 'lodash';
import * as Utils from './utils';

/**
 * Checks if page uses tabs.
 * @param {*} page 
 */
export function pageUsesTabs(page)
{
    const { components } = page;

    return components 
            && components.length > 0 
            && components[0].componentType === 'Tab';
}

/**
 * Finds component based on component key from list of components.
 * @param {Array} components 
 * @param {string} componentKey 
 */
function findFromComponents(components, componentKey)
{
    /** Return component if its key matches componentKey. */
    const getMatchingComponent = (component, componentKey) => 
    {
        if (component.componentKey === componentKey)
        {
            return component;
        }
    }

    for (let i = 0; i < components.length; i++)
    {
        let match = getMatchingComponent(components[i], componentKey);

        if (match)
        {
            return match;    
        }

        if (components[i].children)
        {
            match = findFromComponents(components[i].children, componentKey);
        }

        if (match)
        {
            return match;
        }
    }

    return null;
}

/**
 * Finds the first component with key from given page.
 * Returns null if no match is found.
 * @param {*} page 
 * @param {*} componentKey 
 */
export function findComponentFromPage(page, componentKey)
{
    const { components } = page;

    if (pageUsesTabs(page))
    {
        let match;

        _.forEach(components, tab => 
        {
            if (_.isNil(match))
            {
                match = findFromComponents(tab.tabContent, componentKey);
            }
        })

        return match;
    }

    return findFromComponents(components, componentKey);
}

/**
 * Replaces all component with matching componentKey to substituteComponent.
 * @param {Array} components 
 * @param {object} substituteComponent 
 */
function replaceInComponents(components, substituteComponent)
{
    /** Return substitute component if match, otherwise
     * return component. 
     */
    const handleComponent = (component, componentKey) => 
    {
        if (component.componentKey === componentKey)
        {
            return substituteComponent;
        }

        return component;
    }

    for (let i = 0; i < components.length; i++)
    {
        components[i] = handleComponent(components[i], substituteComponent.componentKey);

        if (components[i].children)
        {
            replaceInComponents(components[i].children, substituteComponent);
        }
    }
}

/**
 * Replace component in page.
 * Modifies existing page object. 
 * Returns modified page object.
 * @param {object} page 
 * @param {object} component New component replacing old one.
 */
export function replaceComponent(page, component)
{
    let { components } = page;

    if (pageUsesTabs(page))
    {
        _.forEach(components, tab => 
        {
            replaceInComponents(tab.tabContent, component);
        })

        return page;
    }

    replaceInComponents(components, component);

    return page;
}

/**
 * Replaces all data found from data object.
 * @param {object} page 
 * @param {string} dataKey 
 */
export function replaceData(page, dataKey, newValue)
{
    const { data } = page;

    const checkObject = (data) => 
    {
        return Utils.fromEntries(Object.entries(data).map(handleEntry))
    }

    /** Replaces entries value with newValue if keys match. 
     * Else if value is object, loop through that and set value as that. 
    */
    const handleEntry = ([key, value]) => 
    {
        if (key === dataKey)
        {
            return [key, newValue];
        }

        if (Utils.isObject(value))
        {
            value = checkObject(value);
        }

        return [key, value];
    }

    page.data = checkObject(data);
    
    return page;
}

/**
 * Returns data object from all datakeys tied to components.
 * @param {*} components 
 * @param {*} pageData 
 */
export function getDataFromComponents(components, pageData)
{   
    let dataObject = {};

    // Check if component has data in pageData and sets it to dataObject
    const setComponentData = (component) => 
    {
        if (_.get(component, 'dataKey', null) === null)
        {
            return;
        }

        if (!_.has(pageData, component.dataKey))
        {
            return;
        }

        dataObject[component.componentKey] = _.get(pageData, component.dataKey, null);
    }

    _.forEach(components, component => 
    {
        if (component.hasOwnProperty('tabContent'))
        {
            _.forEach(component.tabContent, tabComponent => 
            {
               setComponentData(tabComponent); 
            });
        }
        else 
        {
            setComponentData(component);
        }
    });

    return dataObject;
}

/**
 * Parses the active page name from window location.
 */
export function getActivePageName()
{
    const pathname = window.location.pathname;
    return pathname.split('page/')[1];
}

/**
 * Returns data object from actionDataKeys of data, and components included in action.
 * Returned object:
 * {
 *  dataKey: 'data',
 *  dataKey2: 'data',
 *  components: 
 *      {
 *          componentKey: {componentType: 'compType'}
 *      }
 * }
 * 
 * If data's component is container which stores its data as array, value is converted to array.
 * @param {object} data 
 * @param {Array} actionDataKeys
 * @param {Array} components
 * @param {Array} componentsToInclude
 * @param {object} staticData
 */
export function getActionData(data, actionDataKeys, components, componentsToInclude, staticData)
{
    const parameters = {};

    _.forEach(actionDataKeys, dataKey => 
    {
        const value = Utils.findNestedValue(data, dataKey);

        if (!_.isNil(value))
        {
            parameters[dataKey] = value;
        }

        // Data contains only nested objects (except for InputDropdown arrays). If component actually stores data as array, convert object to array.
        const component = findComponentFromPage({ components: components}, dataKey);

        // All data is not bound to a component, check if component is found.
        if (component && component.storeDataAsArray && Utils.isObject(value))
        {
            const array = _.map(Object.keys(value), key => 
            {
                return { [key]: value[key]};
            });

            parameters[dataKey] = array;
        }
    });

    _.merge(parameters, staticData);
    setIncludedComponents(components, parameters, componentsToInclude);

    return parameters;
}

/**
 * Find components to be included in action from components listing and add them to parameters
 * @param {*} components 
 * @param {*} parameters 
 * @param {*} componentsToInclude 
 */
export function setIncludedComponents(components, parameters, componentsToInclude)
{
    // Initialize object if it's not initialized already
    parameters.components = parameters.components || {};

    _.forEach(componentsToInclude, componentKey => 
        {
            parameters.components[componentKey] = findComponentFromPage(
                { components: components },
                componentKey);
        });
}