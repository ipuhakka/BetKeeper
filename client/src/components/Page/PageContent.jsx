import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import TabContainer from 'react-bootstrap/TabContainer';
import Button from '../../components/Page/Button';
import Field from '../../components/Page/Field';
import Table from '../../components/Page/Table';
import StaticTable from '../../components/Page/StaticTable';
import Navbar from '../../components/Navbar/Navbar';
import './customizations.css';

class PageContent extends Component
{
    constructor(props)
    {
        super(props);

        this.state = {
            activeKey: null
        }

        this.handleSelect = this.handleSelect.bind(this);
    }

    handleSelect(key)
    {
        this.setState({ activeKey: key });
    }

    renderTabs(tabs)
    {
        const { activeKey } = this.state;

        const items = tabs.map(tab => {
            return { key: tab.componentKey, text: tab.title };
        })

        const tab = _.find(tabs, tab => tab.componentKey === activeKey) || tabs[0];
        return <div>
            <Navbar 
                items={items}
                handleSelect={this.handleSelect}/>
            <TabContainer>{this.renderComponents(tab.tabContent, 'tab-content')}</TabContainer>
        </div>;
    }

    renderComponents(components, className, depth = 0, dataPath = null)
    {
        const { props } = this;
        
        let completeDataPath = dataPath;
        if (!_.isNil(props.absoluteDataPath))
        {
            completeDataPath = _.join(_.compact([props.absoluteDataPath, dataPath]), '.');
        }

        return <div key={`${className}-${depth}`} className={className}>
            {_.map(components, (component, i) => 
            {
                switch (component.componentType)
                {
                    case 'Container':
                        return this.renderComponents(
                            component.children, 
                            !className.includes('container-div')
                                ? `container-div ${component.customCssClass}` 
                                : `child-container-div child-${i} ${component.customCssClass}`,
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
                                _.compact([completeDataPath, component.dataKey]).join('.'),
                                 null) ||
                                 _.get(props.data, component.dataKey, null)}
                            dataPath={completeDataPath} 
                            {...component} />;

                    case 'Label':
                        return <label key={`label-${i}`}>{component.text}</label>;

                    case 'Table':
                        return <Table onRowClick={props.onTableRowClick} key={`itemlist-${i}`} {...component} />;

                    case 'StaticTable':
                        return <StaticTable componentKey={component.componentKey} key={`custom-table-${i}`} rows={component.rows} header={component.header}/>
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
    data: PropTypes.object,
    absoluteDataPath: PropTypes.string
};

export default PageContent;