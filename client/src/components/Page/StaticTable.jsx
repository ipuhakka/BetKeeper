import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Table from 'react-bootstrap/Table';

class StaticTable extends Component 
{
    renderTableRow(rowData, i)
    {
        const {componentKey, useColumnHeader} = this.props;
        const { cells } = rowData;

        const getStyles = (cell) => 
        {
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
                    key={`td-${i}-${j}`}>{cell.value}</td>;
            })}</tr>;
    }

    render()
    {
        const { rows, header, useColumnHeader } = this.props;

        const tableRows = rows.map((row, i) => 
            {
                return this.renderTableRow(row, i);
            })

        const tableHeader = header
            ? <thead>
                <tr>
                    {header.cells.map((headerCell, i) => {
                        return <th className={`${i === 0 && useColumnHeader ? 'header-column' : ''}`} key={`header-cell-${headerCell.value}`}>{headerCell.value}</th>
                    })}
                </tr>
            </thead>
            : null;

        return <div className='table-div'>
        <Table striped bordered size="sm">
            {tableHeader}
            <tbody>
                {tableRows}
            </tbody>
        </Table>
        </div>;
    }
};

const cellProps = PropTypes.arrayOf(
    PropTypes.shape({
        color: PropTypes.string,
        style: PropTypes.string,
        value: PropTypes.string
    }));

StaticTable.propTypes = {
    componentKey: PropTypes.string,
    header: PropTypes.shape({
            cellProps
        }
    ),
    rows: PropTypes.arrayOf(
        PropTypes.shape({ cells: cellProps })),
    useColumnHeader: PropTypes.bool.isRequired
}

export default StaticTable;