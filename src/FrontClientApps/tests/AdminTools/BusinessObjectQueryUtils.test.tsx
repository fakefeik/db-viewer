import { expect } from "chai";
import { suite, test } from "mocha-typescript";
import { BusinessObjectFieldFilterOperator } from "Domain/Api/DataTypes/BusinessObjectFieldFilterOperator";
import { BusinessObjectFilterSortOrder } from "Domain/Api/DataTypes/BusinessObjectFilterSortOrder";
import { Condition } from "Domain/Api/DataTypes/Condition";
import { Sort } from "Domain/Api/DataTypes/Sort";

import { ConditionsMapper, SortMapper } from "Domain/BusinessObjects/BusinessObjectSearchQueryUtils";
import { QueryStringMapping } from "Domain/QueryStringMapping/QueryStringMapping";
import { QueryStringMappingBuilder } from "Domain/QueryStringMapping/QueryStringMappingBuilder";

interface BusinessObjectSearchQuery {
    conditions: Nullable<Condition[]>;
    sort: Nullable<Sort>;
    count: Nullable<number>;
    offset: Nullable<number>;
    countLimit: Nullable<number>;
    hiddenColumns: Nullable<string[]>;
}

const mapper: QueryStringMapping<BusinessObjectSearchQuery> = new QueryStringMappingBuilder<BusinessObjectSearchQuery>()
    .mapToInteger(x => x.count, "count")
    .mapToInteger(x => x.offset, "offset")
    .mapToInteger(x => x.countLimit, "countLimit")
    .mapToStringArray(x => x.hiddenColumns, "hiddenColumns")
    .mapTo(x => x.sort, new SortMapper("sort"))
    .mapTo(x => x.conditions, new ConditionsMapper(["sort", "count", "offset", "hiddenColumns", "countLimit"]))
    .build();

class BusinessObjectSearchQueryUtils {
    public static parse(search: Nullable<string>): BusinessObjectSearchQuery {
        return mapper.parse(search);
    }

    public static stringify(query: Partial<BusinessObjectSearchQuery>): Nullable<string> {
        return mapper.stringify({
            conditions: null,
            sort: null,
            count: null,
            offset: null,
            countLimit: null,
            hiddenColumns: null,
            ...query,
        });
    }
}

@suite
export class BusinessObjectSearchQueryUtilsTest {
    @test
    public "должен парсить сортировку в простых случаях"() {
        expect(BusinessObjectSearchQueryUtils.parse("?sort=path.to.object:asc")).to.eql({
            conditions: null,
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: {
                path: "path.to.object",
                sortOrder: "Ascending",
            },
        });
        expect(BusinessObjectSearchQueryUtils.parse("?sort=path.to.object%3Aasc")).to.eql({
            conditions: null,
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: {
                path: "path.to.object",
                sortOrder: "Ascending",
            },
        });
        expect(BusinessObjectSearchQueryUtils.parse("?sort=path.to.object")).to.eql({
            conditions: null,
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: {
                path: "path.to.object",
                sortOrder: "Descending",
            },
        });
        expect(BusinessObjectSearchQueryUtils.parse("?sort=")).to.eql({
            conditions: null,
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: null,
        });
        expect(BusinessObjectSearchQueryUtils.parse("?sort=  &a=1")).to.eql({
            conditions: [
                {
                    operator: "Equals",
                    path: "a",
                    value: "1",
                },
            ],
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: null,
        });
        expect(BusinessObjectSearchQueryUtils.parse("?sort=x:1")).to.eql({
            conditions: null,
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: {
                path: "x",
                sortOrder: "Descending",
            },
        });
        expect(BusinessObjectSearchQueryUtils.parse("?sort=:asc")).to.eql({
            conditions: null,
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: null,
        });
    }

    @test
    public "должен переводить в строку"() {
        expect(
            BusinessObjectSearchQueryUtils.stringify({
                sort: {
                    path: "path.to.object",
                    sortOrder: BusinessObjectFilterSortOrder.Ascending,
                },
                conditions: null,
            })
        ).to.eql("?sort=path.to.object%3Aasc");
        expect(
            BusinessObjectSearchQueryUtils.stringify({
                sort: null,
                conditions: null,
            })
        ).to.eql("");
        expect(
            BusinessObjectSearchQueryUtils.stringify({
                sort: {
                    path: null,
                    sortOrder: BusinessObjectFilterSortOrder.Ascending,
                },
                conditions: null,
            })
        ).to.eql("");
    }

    @test
    public "должен парсить массив значений"() {
        expect(BusinessObjectSearchQueryUtils.parse("?Box.Id=%3E123")).to.eql({
            conditions: [
                {
                    path: "Box.Id",
                    value: "123",
                    operator: BusinessObjectFieldFilterOperator.GreaterThan,
                },
            ],
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: null,
        });
        expect(BusinessObjectSearchQueryUtils.parse("?Box.Id=%3D123&Box.Gln=%3D456")).to.eql({
            conditions: [
                {
                    path: "Box.Id",
                    value: "123",
                    operator: BusinessObjectFieldFilterOperator.Equals,
                },
                {
                    path: "Box.Gln",
                    value: "456",
                    operator: BusinessObjectFieldFilterOperator.Equals,
                },
            ],
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
            sort: null,
        });
        expect(BusinessObjectSearchQueryUtils.parse("?Box.Id=%3E123&Box.Gln=%3C321")).to.eql({
            conditions: [
                {
                    path: "Box.Id",
                    value: "123",
                    operator: BusinessObjectFieldFilterOperator.GreaterThan,
                },
                {
                    path: "Box.Gln",
                    value: "321",
                    operator: BusinessObjectFieldFilterOperator.LessThan,
                },
            ],
            sort: null,
            count: null,
            countLimit: null,
            offset: null,
            hiddenColumns: null,
        });
        expect(BusinessObjectSearchQueryUtils.parse("?offset=20")).to.eql({
            count: null,
            countLimit: null,
            offset: 20,
            hiddenColumns: null,
            sort: null,
            conditions: null,
        });
        expect(
            BusinessObjectSearchQueryUtils.parse(
                "?offset=20&count=100&sort=Box.Id:asc&Box.Id=%3E10&LastModificationDateTime=%3E%3D10"
            )
        ).to.eql({
            count: 100,
            countLimit: null,
            offset: 20,
            hiddenColumns: null,
            sort: {
                path: "Box.Id",
                sortOrder: "Ascending",
            },
            conditions: [
                {
                    path: "Box.Id",
                    value: "10",
                    operator: "GreaterThan",
                },
                {
                    path: "LastModificationDateTime",
                    value: "10",
                    operator: "GreaterThanOrEquals",
                },
            ],
        });
    }

    @test
    public "должен переводить в строку объект"() {
        expect(
            BusinessObjectSearchQueryUtils.stringify({
                count: 100,
                offset: 20,
                sort: {
                    path: "Box.Id",
                    sortOrder: BusinessObjectFilterSortOrder.Ascending,
                },
                conditions: [
                    {
                        path: "Box.Id",
                        value: "10",
                        operator: BusinessObjectFieldFilterOperator.GreaterThan,
                    },
                    {
                        path: "LastModificationDateTime",
                        value: "10",
                        operator: BusinessObjectFieldFilterOperator.GreaterThanOrEquals,
                    },
                ],
            })
        ).to.eql("?count=100&offset=20&sort=Box.Id%3Aasc&Box.Id=%3E10&LastModificationDateTime=%3E%3D10");
        expect(
            BusinessObjectSearchQueryUtils.stringify({
                count: 20,
                offset: 1580,
                conditions: [
                    {
                        path: "Box.Gln",
                        value: "10",
                        operator: BusinessObjectFieldFilterOperator.LessThan,
                    },
                    {
                        path: "LastModificationDateTime",
                        value: "13263165",
                        operator: BusinessObjectFieldFilterOperator.LessThanOrEquals,
                    },
                ],
            })
        ).to.eql("?count=20&offset=1580&Box.Gln=%3C10&LastModificationDateTime=%3C%3D13263165");
    }
}
