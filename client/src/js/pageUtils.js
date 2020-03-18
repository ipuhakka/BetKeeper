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