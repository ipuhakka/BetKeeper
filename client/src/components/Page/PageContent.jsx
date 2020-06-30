import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import Tabs from 'react-bootstrap/Tabs';
import Tab from 'react-bootstrap/Tab';
import Button from '../../components/Page/Button';
import Field from '../../components/Page/Field';
import Table from '../../components/Page/Table';

class PageContent extends Component
{

    renderTabs(tabs)
    {
        return <Tabs defaultActiveKey={tabs[0].componentKey}>
            {_.map(tabs, (tab, i) => {
                return <Tab key={`tab-${i}`} eventKey={tab.componentKey} title={tab.title}>
                    {this.renderComponents(tab.tabContent, 'tab-content')}
                </Tab>  
            })}
        </Tabs>;
    }

    renderComponents(components, className, depth = 0, dataPath = null)
    {
        const { props } = this;

        return <div key={`${className}-${depth}`} className={className}>
            {_.map(components, (component, i) => 
            {
                switch (component.componentType)
                {
                    case 'Container':
                        return this.renderComponents(
                            component.children, 
                            className !== 'container-div' ? 'container-div' : `child-container-div child-${i}`,
                            depth + 1,
                            /** Datapath */
                            _.compact([dataPath, component.componentKey]).join('.')
                        );

                    case 'Button':
                        return <Button 
                            key={`button-${component.action}-${i}`}
                            onClick={props.getButtonClick(component)} 
                            {...component} />;

                    case 'Field':
                        return <Field 
                            onChange={props.onFieldValueChange}
                            onHandleDropdownServerUpdate={props.onHandleDropdownServerUpdate}
                            key={`field-${component.componentKey}`} 
                            type={component.fieldType} 
                            componentKey={component.componentKey}
                            initialValue={_.get(
                                props.data, 
                                _.compact([dataPath, component.dataKey]).join('.'),
                                 null) ||
                                 _.get(props.data, component.dataKey, null)}
                            dataPath={dataPath} 
                            {...component} />;

                    case 'Table':
                        return <Table onRowClick={props.onTableRowClick} key={`itemlist-${i}`} {...component} />;

                    default:
                        throw new Error(`Component type ${component.componentType} not implemented`);
                }
            })}
        </div>;
    }

    render()
    {
        const { components } = this.props;

        return components 
            && components.length > 0 
            && components[0].componentType === 'Tab'
            ? this.renderTabs(components)
            : this.renderComponents(components, 'page-content');
    }
};

PageContent.propTypes = {
    components: PropTypes.array,
    className: PropTypes.string,
    getButtonClick: PropTypes.func.isRequired,
    onTableRowClick: PropTypes.func,
    onFieldValueChange: PropTypes.func.isRequired,
    onHandleDropdownServerUpdate: PropTypes.func,
    data: PropTypes.object
};

export default PageContent;