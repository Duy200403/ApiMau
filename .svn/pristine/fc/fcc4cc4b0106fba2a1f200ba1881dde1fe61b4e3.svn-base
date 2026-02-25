
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ApiWebsite.Helper
{
    public enum AccountType
    {
        Customer = 0,
        Manager = 1
    }

    public enum ActionType
    {
        Remove = 0,
        Add = 1
    }

    public enum DataType
    {
        category = 0,
        news = 1,
        personnel = 2,
        report = 3,
        introduce = 4,
        termOfUser = 5,
        contact = 6
    }

    public enum FileAliAs
    {
        images = 0,
        document = 1,
        videos = 2,
        // wellknown=3
    }

    public enum ErrorModelPropertyName
    {
        content = 0,
    }

    public enum MyMethod
    {
        Get,
        Post,
        Put,
        Delete
    }

    public enum PostStatus
    {
        // all = 0,
        pending = 1,
        release = 2,
        refuse = 3,
        draft = 4
    }

    public enum OS
    {
        android = 1,
        ios = 2,
    }

    public enum Role
    {
        admin = 1, // thao tác tất cả chức năng
        manager = 2, // thao tác tất cả chức năng
        publisher = 3, // phê duyệt, kiểm định, tạo ,xem , xóa các bài viết
        editor = 4,  // tạo, sửa, xóa bài viết :  document private, document publish
                     // customer = 5 // khách hàng chỉ được đọc tin bài viết
        general = 7,
        xembaocao = 8
    }

    public enum BookStatus
    {
        waitingForApprove = 1, // chờ phê duyệt book thiết bị
        approvedAndBooked = 2, // phê duyệt và coi như book
        finishBooking = 3, // không có thiết bị (Trường hợp đang sửa hoặc đã không có trong bệnh viện), cũng không được book
        unApprove = 4, // đã hủy phê duyệt
        notApprove = 5, // đã không phê duyệt
    }

    public enum DeviceStatus
    {
        // deleted = 0, // đã xóa
        available = 1, // đã book thiết bị
        notAvailable = 2, // không có thiết bị (Trường hợp đang sửa hoặc đã không có trong bệnh viện), cũng không được book
        currentlyStopped = 3 // tạm dừng hoạt động thiết bị
    }

    public enum LogLevelWebInfo
    {
        trace = 0,
        information = 1,
        error = 2,
    }



    public enum DataTypeCategory
    {
        information = 1,
        video = 2,
        image = 3,
        introduce = 4,
        pdf = 5,
        person = 6,
        livestream = 7,
        home = 8,
        contact = 9
    }

    public enum DataTypeSex
    {
        male = 1,
        female = 2,
        other = 3
    }

    public enum DataTypeVideo
    {
        none = 0,
        all = 1,
        program = 2,
        tvc = 3,
    }

    public enum TrangThaiDeNghi
    {
        daXuLy = 1,
        dangXuLy = 2,
        chuaXuLy = 3,
    }

    public enum LoaiXuLy
    {
        tuXuLy = 1,
        doiTacXuLy = 2,
        koChon = 3
    }

    public enum Loai
    {
        them = 1,
        sua = 2,
        xoa = 3,
        chon = 4,
        koChon = 5
    }

    public enum LoaiNoiDung
    {
        noidungdenghi = 1,
        chinhsuabaohiem = 2,
        koChon = 3
    }

    public enum SortBy
    {
        ngayDeNghi = 1,
        tienChenhLech = 2,
    }

    public enum DynamicType
    {
        html = 0,
        text = 1,
        image = 2,
        video = 3,
        script = 5,
        model = 6
    }

    public enum SortDirection
    {
        DESC,
        ASC
    }
    public enum typeError
    {
        UserName = 0,
        Email = 1,
        Code = 2
    }

    public enum PostReportType
    {
        ByStatus,
        ByCreatedDate,
        ByPublicDate,
        DataType,
        Author,
        Category

    }
    public enum AdvertisingAnalysisReportType
    {
        ByCategoryId,
        ByImageThumb,
        ByDate

    }
}