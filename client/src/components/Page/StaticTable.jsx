import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Table from 'react-bootstrap/Table';
import DynamicColorText from './DynamicColorText';

class StaticTable extends Component 
{
    renderTableRow(rowData, i)
    {
        const {componentKey, useColumnHeader} = this.props;
        const { cells } = rowData;

        const getStyles = (cell) => 
        {
            if (cell.items)
            {
                // Dynamically coloured content, ignore styling
                return null;
            }

            return {
                color: cell.color,
                fontWeight: cell.style === 'Bold'
                    ? 600
                    : 'normal'
            };
        }

        return <tr key={`tr-${componentKey}-${i}`}>{cells.map((cell, j) => 
            {
                return <td
                    className={`${j === 0 && useColumnHeader ? 'header-column' : ''} ${i % 2 === 0 ? 'gray' : ''}`}
                    style={getStyles(cell)} 
                    key={`td-${i}-${j}`}>{cell.items 
                        ? <DynamicColorText {...cell} /> 
                        :cell.value}</td>;
            })}</tr>;
    }

    render()
    {
        const { rows, header, useColumnHeader, useStickyHeader } = this.props;

        const tableRows = rows.map((row, i) => 
            {
                return this.renderTableRow(row, i);
            })

        const tableHeader = header
            ? <thead>
                <tr>
                    {header.cells.map((headerCell, i) => {
                        return <th className={`${useStickyHeader ? 'static-table-sticky-head-cell' : ''}${i === 0 && useColumnHeader ? ' header-column' : ''}`} key={`header-cell-${headerCell.value}`}>{headerCell.value}</th>
                    })}
                </tr>
            </thead>
            : null;

        return <div className='static-table-div'>
        <Table striped bordered size="sm">
            {tableHeader}
            <tbody>
                {tableRows}
            </tbody>
        </Table>
        </div>;
    }
};

const cellProps = PropTypes.shape({
        color: PropTypes.string,
        style: PropTypes.string,
        value: PropTypes.string
    });

const coloredCellProps = PropTypes.shape({
    items: PropTypes.arrayOf({
        value: PropTypes.string.isRequired,
        color: PropTypes.string.isRequired
    })
});

StaticTable.propTypes = {
    componentKey: PropTypes.string,
    header: cellProps,
    rows: function(props, propName, componentName) 
    {
        const rows = props[propName];
        if (!Array.isArray(rows))
        {
            return new Error('rows is not an array');
        }

        if (!PropTypes.arrayOf(cellProps) || !PropTypes.arrayOf(coloredCellProps))
        {
            return new Error('Rows props invalid');
        }
      },
    useColumnHeader: PropTypes.bool.isRequired,
    useStickyHeader: PropTypes.bool.isRequired
}

export default StaticTable;