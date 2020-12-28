import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import ListGroupRB from 'react-bootstrap/ListGroup';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import './ListGroup.css';
import * as Utils from '../../js/utils';
import * as PageUtils from '../../js/pageUtils';
import PageContent from './PageContent';
import * as PageActions from '../../actions/pageActions';

class ListGroup extends Component
{
    constructor(props)
    {
        super(props);

        this.state = {
            selected: []
        }

        this.handleGroupItemClick = this.handleGroupItemClick.bind(this);
    }

    /**
     * Handle group item click
     * @param {*} itemIdentifier 
     */
    handleGroupItemClick(itemIdentifier)
    {
        const { selected } = this.state;
        const { mode, onSelect, dataPath, componentKey } = this.props;

        if (mode === 'DisplayOnly')
        {
            return;
        }

        /** Use selected state for both selectable and expandable type listgroup  */
        const newSelected = _.xor(selected, [itemIdentifier]);
        this.setState(
        {
            selected: newSelected
        });

        /** Join dataPath and component key */
        const dataKey = _.compact([dataPath, componentKey]).join('.');

        if (mode === 'Selectable')
        {
            /** Call select event only for selectable */
            onSelect(dataKey, newSelected);
        }
    }

    /**
     * Gets text shown for item.
     * @param {object} itemDefinition Object with fieldKey and fieldType string properties
     * @param {object} itemData
     */
    getItemText(itemDefinition, itemData)
    {
        let value;

        if (itemDefinition.fieldType === 'DateTime')
        {
            value = Utils.formatDateTime(itemData[itemDefinition.fieldKey])
        }
        else 
        {
            value = itemData[itemDefinition.fieldKey];
        }

        return _.compact([itemDefinition.fieldLegend, value]).join(': ');
    }

    /**
     * Returns page content for expanded item.
     * @param {*} dataItem 
     */
    getItemContent(dataItem)
    {
        const { itemActions, itemFields, componentKey, keyField } = {...this.props};

        return itemActions.map(actionButton =>
        {
            return {
                ...actionButton,
                actionDataKeys: actionButton.actionDataKeys.concat([`listGroup-${componentKey}-${dataItem[keyField]}`])
            };
        }).concat(itemFields);
    }

    render()
    {
        const { data, headerItems, smallItems, keyField, mode, componentKey } = this.props;

        const items = data.map((dataItem, i) =>
            {
                const itemContent = this.getItemContent(dataItem);
                return <div key={`${keyField}-div-${i}`}>
                        <ListGroupItem 
                        action={mode !== 'DisplayMode'}
                        variant={this.state.selected.includes(dataItem[keyField]) ? 'info': null}
                        onClick={() => 
                        {
                            this.handleGroupItemClick(dataItem[keyField]);
                        }}
                        key={`${keyField}-header-${i}`}>
                        <div>{headerItems.map(item => this.getItemText(item, dataItem)).join(' ')}</div>
                        <div className='small-div'>{(smallItems || []).map(item => this.getItemText(item, dataItem)).join(' ')}</div>
                    </ListGroupItem>
                    <div className={`listGroupItemBody${this.state.selected.includes(dataItem[keyField]) ? ' visible' : ''}`}>
                        <PageContent 
                            onFieldValueChange={(dataPath, newValue) => 
                            {
                                PageActions.onDataChange(PageUtils.getActivePageName(), dataPath, newValue);
                            }}
                            components={itemContent}
                            getButtonClick={this.props.onAction}
                            className='listGroup-content'
                            absoluteDataPath={`listGroup-${componentKey}-${dataItem[keyField]}`}
                            data={dataItem}/>
                    </div>
                </div>;
            });

        return <ListGroupRB>{items}</ListGroupRB>;
    }
};

ListGroup.propTypes = {
    data: PropTypes.arrayOf(PropTypes.object).isRequired,
    keyField: PropTypes.string.isRequired,
    headerItems: PropTypes.arrayOf(
        PropTypes.shape({
            fieldKey: PropTypes.string.isRequired,
            fieldType: PropTypes.string.isRequired,
            fieldLegend: PropTypes.string
        })).isRequired,
    smallItems: PropTypes.arrayOf(
        PropTypes.shape({
            fieldKey: PropTypes.string.isRequired,
            fieldType: PropTypes.string.isRequired,
            fieldLegend: PropTypes.string
        })),
    mode: PropTypes.string.isRequired,
    componentKey: PropTypes.string,
    dataPath: PropTypes.string,
    onSelect: PropTypes.func.isRequired,
    itemActions: PropTypes.array,
    itemFields: PropTypes.array.isRequired,
    onAction: PropTypes.func
}

export default ListGroup;