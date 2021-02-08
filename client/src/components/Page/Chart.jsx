import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import { BarChart, Bar, PieChart, Pie, CartesianGrid, XAxis, YAxis, ResponsiveContainer, Tooltip, Legend } from 'recharts';
import Dropdown from './Dropdown';
import Table from 'react-bootstrap/Table';
import * as utils from '../../js/utils';
import './Page.css';

/** Color palette to color data items */
const colors = [
    '#000000',
    '#fff100',
    '#ff8c00',
    '#e81123',
    '#ec008c',
    '#68217a',
    '#00188f',
    '#00bcf2',
    '#00b294',
    '#009e49'
];

class Chart extends Component
{
    constructor(props)
    {
        super(props);

        this.state = {
            activeDataField: props.dataFields[0],
            activeChart: 'bar',
            data: []
        };

        this.onDataFieldChange = this.onDataFieldChange.bind(this);
        this.renderCustomToolTip = this.renderCustomToolTip.bind(this);
    }

    componentDidUpdate()
    {
        const { data } = {...this.props};
        if (!_.isEqual(this.props.data, this.state.data))
        {
            // Set color for item
            for (var i = 0; i < data.length; i++)
            {
                data[i].fill = colors[i % colors.length];
            }
            this.setState({ data: data});
        }
    }

    onDataFieldChange(componentKey, newActiveKey)
    {
        const { dataFields } = this.props;
        this.setState({ activeDataField: dataFields.find(field => field.fieldKey === newActiveKey)});
    }

    renderChartSelection()
    {
        const { activeChart } = this.state;
        const options = [
            { key: 'bar', value: 'Bar chart', initialValue: activeChart === 'bar' },
            { key: 'pie', value: 'Pie chart', initialValue: activeChart === 'pie' },
            { key: 'table', value: 'Table', initialValue: activeChart === 'table' }
        ];

        // Handle chart selection change
        const onChange = (componentKey, newValue) => 
        {
            this.setState({ activeChart: newValue });
        }

        return <Dropdown 
            options={options}
            componentKey='chart-selection-dropdown'
            onChange={onChange}
            className='chart-selection'/>;
    }

    /**
     * Render custom label for pie chart. Shows active field's value
     * @param {*} item 
     */
    renderCustomLabel(item)
    {
        const { activeDataField } = this.state;
        return <text
        fill={item.fill}
        x={item.x}
        y={item.y}
        stroke='none'
        alignmentBaseline='middle'
        className='recharts-text recharts-pie-label-text'
        textAnchor='end'>
        <tspan x={item.x} textAnchor={item.textAnchor} dy='0em'>{item[activeDataField.fieldKey]}</tspan>
     </text>;
    }

    /** Renders a custom tooltip for bar chart */
    renderCustomToolTip(props)
    {
        if (!props.active)
        {
            return null;
        }

        const { keyField } = this.props;
        const { activeDataField } = this.state;
        const { payload } = props.payload[0];
        const tooltip = `${payload[keyField.fieldKey]}: ${payload[activeDataField.fieldKey]}`
        return <div className='barchart-custom-tooltip'>{tooltip}</div>;
    }

    renderTable()
    {
        const { keyField, dataFields, data } = this.props;

        const fields = [keyField].concat(dataFields);
        const getHeaders = () =>
        {

            return <tr>
                {_.map(fields, column => {
                    return <th key={`table-header-${column.fieldKey}`}>{column.fieldLegend}</th>;
                })}
            </tr>;
        };

        const getRows = () => 
        {
            return _.map(data, (dataRow, i) =>
            {
                const cells = _.map(fields, (column, i) =>
                {
                    const value = column.fieldType === 'DateTime'
                        ? utils.formatDateTime(dataRow[column.fieldKey])
                        : dataRow[column.fieldKey];

                    return <td key={`data-row-td-${i}-key`}>{value}</td>;
                })

                return <tr key={`data-row-tr-${i}-key`}>{cells}</tr>;
            });
        }

        return <Table className='page-chart-table' hover responsive striped bordered>
            <thead>
                {getHeaders()}
            </thead>
            <tbody>
                {getRows()}
            </tbody>
        </Table>;
    }

    renderChart()
    {
        const { activeChart, activeDataField, data } = this.state;
        const { keyField } = this.props;

        switch (activeChart)
        {
            case 'bar':
                return <ResponsiveContainer height={"99%"}>
                <BarChart barSize={20} barGap={2} data={data}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis interval={0} dataKey={activeDataField.fieldKey}/>
                    <YAxis/>
                    <Tooltip content={this.renderCustomToolTip}/>
                    <Bar name={activeDataField.fieldLegend} dataKey={activeDataField.fieldKey}/>
                    <Legend payload={
                        data.map(
                            item => ({
                                type: "square",
                                value: item[keyField.fieldKey],
                                color: item.fill
                            }))
                    }/>
                </BarChart>
            </ResponsiveContainer>;

            case 'pie':
                if (data.some(item => item[activeDataField.fieldKey] < 0))
                {
                    return <div>Property has negative values, cannot show pie chart</div>;
                }
                
                return <ResponsiveContainer>
                    <PieChart>
                        <Pie
                            isAnimationActive={false}
                            data={data}
                            dataKey={activeDataField.fieldKey} 
                            nameKey={keyField.fieldKey} 
                            cx="50%" 
                            cy="50%" 
                            innerRadius={60}
                            outerRadius={80}
                            label={item => this.renderCustomLabel(item)} />
                        <Legend />
                        <Tooltip />
                  </PieChart>
                </ResponsiveContainer>;

            case 'table':
                return this.renderTable();

            default:
                throw new Error(`${activeChart} not implemented`);
        }
    }

    render()
    {
        const { activeDataField, activeChart } = this.state;
        const { dataFields } = this.props;

        const options = dataFields.map(field => {
            return { key: field.fieldKey, value: field.fieldLegend, initialValue: field.fieldKey === activeDataField.fieldKey };
        });
        
        return <div>
            {this.renderChartSelection()}
            {activeChart !== 'table' && <Dropdown 
                options={options}
                componentKey={`chart-field-selection-dropdown`}
                onChange={this.onDataFieldChange}/>}
            <div className={`${activeChart !== 'table' ? 'page-chart' : ''}`}>
            {this.renderChart()}
        </div>
    </div>;
    }
};

Chart.propTypes = {
    data: PropTypes.array.isRequired,
    keyField: PropTypes.shape({
        fieldKey: PropTypes.string.isRequired,
        fieldType: PropTypes.string.isRequired,
        fieldLegend: PropTypes.string.isRequired
    }).isRequired,
    dataFields: PropTypes.arrayOf(
        PropTypes.shape({
            fieldKey: PropTypes.string.isRequired,
            fieldType: PropTypes.string.isRequired,
            fieldLegend: PropTypes.string.isRequired
        })),
};

export default Chart;