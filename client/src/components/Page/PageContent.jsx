import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import TabContainer from 'react-bootstrap/TabContainer';
import Button from '../../components/Page/Button';
import Field from '../../components/Page/Field';
import Table from '../../components/Page/Table';
import StaticTable from '../../components/Page/StaticTable';
import Chart from './Chart';
import Navbar from '../../components/Navbar/Navbar';
import ListGroup from './ListGroup';
import CardMenu from './CardMenu';
import Panel from './Panel';
import './customizations.css';

class PageContent extends Component
{
    constructor(props)
    {
        super(props);

        this.state = {
            activeKey: null,
            invalidFields: []
        }

        this.handleSelect = this.handleSelect.bind(this);
        this.onFieldValueChange = this.onFieldValueChange.bind(this);
    }

    /**
     * Field value change function. Keeps track of erroneous fields
     * @param {string} dataPath 
     * @param {*} newValue 
     * @param {*} isValid 
     */
    onFieldValueChange(dataPath, newValue, isValid)
    {
        const { props } = this;
        const invalidFields = [...this.state.invalidFields];

        if (!isValid)
        {
            if (!invalidFields.includes(dataPath))
            {
                invalidFields.push(dataPath);
                this.setState({ invalidFields });
            }
        }
        else
        {
            if (invalidFields.includes(dataPath))
            {
                const index = invalidFields.findIndex(item => item === dataPath);
                invalidFields.splice(index, 1);
                this.setState({ invalidFields });
            }

            props.onFieldValueChange(dataPath, newValue);
        }
    }

    handleSelect(key)
    {
        this.setState({ activeKey: key });
    }

    /**
     * Get tedt for label component
     * @param {*} label 
     */
    getLabelText(label)
    {
        if (label.isDate)
        {
            return new Date(label.text).toLocaleString();
        }

        return label.text;
    }

    renderTabs(tabs)
    {
        const { activeKey } = this.state;

        const items = tabs.map(tab => {
            return { key: tab.componentKey, text: tab.title };
        })

        const tab = _.find(tabs, tab => tab.componentKey === activeKey) || tabs[0];
        return <div className='page-tab-content-div'>
            <Navbar 
                items={items}
                handleSelect={this.handleSelect}/>
            <TabContainer className='tab-container'>{this.renderComponents(tab.tabContent, 'tab-content')}</TabContainer>
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
                            className={component.customCssClass}
                            onClick={props.getButtonClick(component)}
                            invalidFields={this.state.invalidFields.length > 0}
                            {...component} />;

                    case 'Field':
                        return <Field 
                            onChange={this.onFieldValueChange}
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
                            existingSelections={props.existingSelections} 
                            {...component} />;

                    case 'Label':
                        return <label className='page-content-label' key={`label-${i}`}>{this.getLabelText(component)}</label>;

                    case 'Table':
                        return <Table onRowClick={props.onTableRowClick} key={`itemlist-${i}`} {...component} />;

                    case 'StaticTable':
                        return <StaticTable 
                            componentKey={component.componentKey}
                            key={`custom-table-${i}`}
                            rows={component.rows}
                            header={component.header}
                            useColumnHeader={component.useColumnHeader}
                            useStickyHeader={component.useStickyHeader} />
                    
                    case 'ListGroup':
                        return <ListGroup
                            key={component.componentKey}
                            data={component.data} 
                            keyField={component.keyField} 
                            headerItems={component.headerItems}
                            smallItems={component.smallItems}
                            mode={component.mode}
                            componentKey={component.componentKey}
                            onSelect={props.onFieldValueChange}
                            onAction={props.getButtonClick}/>;

                    case 'Chart':
                        return <Chart 
                            key={component.componentKey}
                            keyField={component.keyField}
                            dataFields={component.dataFields}
                            data={component.data}/>;

                    case 'CardMenu':
                        return <CardMenu 
                        key={component.componentKey}
                        cards={component.cards} 
                        onClick={props.getButtonClick({ buttonType: 'Navigation' })} />;

                    case 'Panel':
                        return <Panel
                            key={component.componentKey}
                            legend={component.legend}>
                            {this.renderComponents(
                                component.children, 
                                `${className} ${component.customCssClass}`,
                                depth + 1,
                                dataPath)}
                        </Panel>;

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
    absoluteDataPath: PropTypes.string,
    /** Input drodown values lists for copying an existing options list */
    existingSelections: PropTypes.object
};

export default PageContent;