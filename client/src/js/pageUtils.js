import _ from 'lodash';

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
 * Finds a component with key from given page.
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
                match = _.find(tab.tabContent, component => component.componentKey === componentKey);
            }
        })

        if (_.isNil(match))
        {
            return null;
        }

        return match;
    }

    const component = components.find(component => component.componentKey === componentKey);

    if (_.isNil(component))
    {
        return null;
    }

    return component;
}

/**
 * Replace component in page.
 * Modifies existing page object. 
 * Returns modified page object.
 * @param {object} page 
 * @param {object} component 
 */
export function replaceComponent(page, component)
{
    const { components } = page;
    const componentKey = component.componentKey;

    if (pageUsesTabs(page))
    {
        _.forEach(components, tab => 
        {
            const componentIndex = _.findIndex(tab.tabContent, component => component.componentKey === componentKey);
            
            if (componentIndex !== -1)
            {
                tab.tabContent[componentIndex] = component;
            }
        })

        return page;
    }

    const componentIndex = components.findIndex(component => component.componentKey === componentKey);

    if (componentIndex !== -1)
    {
        components[componentIndex] = component;
    }
    
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