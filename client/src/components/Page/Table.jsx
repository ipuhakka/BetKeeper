import React, { Component } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import _ from 'lodash';
import RBTable from 'react-bootstrap/Table';
import * as utils from '../../js/utils';
import './Table.css';

class Table extends Component
{
    getHeaders()
    {
        const { columns } = this.props;

        return <tr>
            {_.map(columns, columnField => {
                return <th key={`table-header-${columnField.key}`}>{utils.camelCaseToText(columnField.key)}</th>;
            })}
        </tr>
    }

    getRows()
    {
        const { data, dataKey, navigationKey, onRowClick, columns } = this.props;

        const onClick = (rowIndex) => 
        {
            if (_.isNil(navigationKey))
            {
                return;
            }

            const pathname = window.location.pathname;

            const itemKey = data[dataKey][rowIndex][navigationKey];

            onRowClick(`${pathname}/${itemKey}`);
        }

        return _.map(data[dataKey], (dataRow, i) =>
        {
            const cells = _.map(columns, (columnField, i) =>
            {
                const value = columnField.dataType === 'DateTime'
                    ? utils.formatDateTime(dataRow[columnField.key])
                    : dataRow[columnField.key];

                return <td key={`data-row-td-${i}-key`}>{value}</td>;
            })

            return <tr onClick={() =>
            {
                onClick(i);
            }} 
            key={`data-row-tr-${i}-key`}>{cells}</tr>;
        });
    }

    render()
    {
        const { dataKey, data } = this.props;

        if (_.isNil(data[dataKey]))
        {
            throw new Error(`Specified data key ${dataKey} not found`);
        }

        return <RBTable hover responsive striped bordered>
            <thead>
                {this.getHeaders()}
            </thead>
            <tbody>
                {this.getRows()}
            </tbody>
        </RBTable>
    }
};

Table.propTypes = {
    dataKey: PropTypes.string.isRequired,
    data: PropTypes.object,
    columns: PropTypes.arrayOf(PropTypes.shape({
        key: PropTypes.string.isRequired,
        dataType: PropTypes.string.isRequired
    })),
    navigationKey: PropTypes.string,
    onRowClick: PropTypes.func.isRequired
};

const mapStateToProps = (state) => 
{
    const pathname = window.location.pathname;
    // Parse page name from path
    const pageKey = pathname.substring(pathname.lastIndexOf('/') + 1);

    return {
        data: _.find(state.pages.pages, page =>
            page.pageKey === pageKey).data || {} 
    };
};

export default connect(mapStateToProps)(Table);