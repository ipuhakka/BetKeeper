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
                match = _.find(tab.tabContent, component => component.key === componentKey);
            }
        })

        if (_.isNil(match))
        {
            return null;
        }

        return match;
    }

    const component = components.find(component => component.key === componentKey);

    if (_.isNil(component))
    {
        return null;
    }

    return component;
}

/**
 * Replace component in page. 
 * Returns the new list of components.
 * @param {object} page 
 * @param {object} component 
 */
export function replaceComponent(page, component)
{
    const { components } = page;
    const componentKey = component.key;

    if (pageUsesTabs(page))
    {
        _.forEach(components, tab => 
        {
            const componentIndex = _.findIndex(tab.tabContent, component => component.key === componentKey);
            
            if (componentIndex !== -1)
            {
                tab.tabContent[componentIndex] = component;
            }
        })

        return components;
    }

    const componentIndex = components.findIndex(component => component.key === componentKey);

    if (componentIndex !== -1)
    {
        components[componentIndex] = component;
    }
    
    return components;
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

        dataObject[component.key] = _.get(pageData, component.dataKey, null);
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