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
        return <Tabs defaultActiveKey={tabs[0].key}>
            {_.map(tabs, (tab, i) => {
                return <Tab key={`tab-${i}`} eventKey={tab.key} title={tab.title}>
                    {this.renderComponents(tab.tabContent, 'tab-content')}
                </Tab>  
            })}
        </Tabs>;
    }

    renderComponents(components, className)
    {
        const { props } = this;

        return <div key={className} className={className}>
            {_.map(components, (component, i) => 
            {
                switch (component.componentType)
                {
                    case 'Container':
                        return this.renderComponents(component.children, 'container-div');

                    case 'Button':
                        return <Button 
                            key={`button-${component.action}`}
                            onClick={props.getButtonClick(component)} 
                            {...component} />;

                    case 'Field':
                        return <Field 
                            onChange={props.onFieldValueChange}
                            key={`field-${component.key}`} 
                            type={component.fieldType} 
                            fieldKey={component.key}
                            initialValue={_.get(props.data, component.dataKey, '')} 
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
    data: PropTypes.object
};

export default PageContent;