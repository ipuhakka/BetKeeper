import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import ListGroupRB from 'react-bootstrap/ListGroup';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import './ListGroup.css';
import * as Utils from '../../js/utils';

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

        if (mode === 'Selectable')
        {
            const newSelected = _.xor(selected, [itemIdentifier]);
            this.setState(
            {
                selected: newSelected
            });

            /** Join dataPath and component key */
            const dataKey = _.compact([dataPath, componentKey]).join('.');
            onSelect(dataKey, newSelected);
        }
    }

    /**
     * Gets value shown for item.
     * @param {object} itemDefinition Object with fieldKey and fieldType string properties
     * @param {object} itemData
     */
    getItemValue(itemDefinition, itemData)
    {
        if (itemDefinition.fieldType === 'DateTime')
        {
            return Utils.formatDateTime(itemData[itemDefinition.fieldKey])
        }

        return itemData[itemDefinition.fieldKey];
    }

    render()
    {
        const { data, headerItems, smallItems, keyField, mode } = this.props;

        const items = data.map((dataItem, i) =>
            {
                return <ListGroupItem 
                    action={mode !== 'DisplayMode'}
                    variant={this.state.selected.includes(dataItem[keyField]) ? 'info': null}
                    onClick={() => 
                    {
                        this.handleGroupItemClick(dataItem[keyField]); 
                    }}
                    key={`${keyField}-${i}`}>
                    <div>{headerItems.map(item => this.getItemValue(item, dataItem)).join(' ')}</div>
                    <div className='small-div'>{(smallItems || []).map(item => this.getItemValue(item, dataItem)).join(' ')}</div>
                </ListGroupItem>;
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
            fieldType: PropTypes.string.isRequired
        })).isRequired,
    smallItems: PropTypes.arrayOf(
        PropTypes.shape({
            fieldKey: PropTypes.string.isRequired,
            fieldType: PropTypes.string.isRequired
        })),
    mode: PropTypes.string.isRequired,
    componentKey: PropTypes.string,
    dataPath: PropTypes.string,
    onSelect: PropTypes.func.isRequired
}

export default ListGroup;