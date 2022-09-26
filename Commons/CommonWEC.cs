using System.Text.RegularExpressions;

namespace BWM.Commons
{
    public class CommonWEC
    {
        public static int GetHandleType(string moduleName, string typeName, string content)
        {
            var name = StripTagsRegex(moduleName);

            if (name.StartsWith("KQKL MR") || name.StartsWith("KQKLMR"))
                //switch (typeName)
                //{
                //    case "Báo cáo tiến độ KQKL MR":
                //    case "Công việc KQKLMR quá hạn":
                //        return 11;
                //    default:
                //        return 11;
                //}
                return 11;
            if (name.StartsWith("KQKL"))
                switch (typeName)
                {
                    case "Xác nhận xử lý công việc KQKL thay nhân viên nghỉ đột xuất":
                        return 21;
                    default:
                        return 11;
                }
            if (name.StartsWith("eTask"))
                switch (typeName)
                {
                    case "Xác nhận phát phiếu KQKLMR và đề nghị chốt số lượng KQKL MR cần đăng ký cho các thành viên tham gia":
                    case "Xác nhận gửi biên bản sau khi họp":
                    case "Xác nhận việc họp giao ban offline":
                        return 21;
                    case "Xác nhận sau cuộc họp":
                    case "Cảnh báo đặt lịch họp nhiều ngày/tuần liên tiếp":
                    case "Cảnh báo đặt lịch họp":
                    case "Cảnh báo vi phạm lựa chọn thư ký cuộc họp":
                        return 11;
                    case "Xác nhận số lượng KQKL MR cần đăng ký sau cuộc họp":
                        return 3;

                    default:
                        return 11;
                }

            if (name.StartsWith("BNI"))
                //switch (typeName)
                //{
                //    case "Cảnh báo BNI":
                //    case "Cảnh báo của hệ thống BNI":
                //    case "Backup":
                //    case "Ping Telnet":
                //    case "Sensor":
                //    case "Monitor Port":
                //    case "Folder":
                //    case "Resource":
                //    case "Windows Update":
                //    case "Website":
                //        return 11;
                //    default:
                //        return 11;
                //}
                return 11;

            switch (name)
            {
                case "PPC":
                    switch (typeName)
                    {
                        case "Xác nhận cảnh báo":
                        case "Xác nhận rà soát PPC":
                        case "PPC nhắc việc định kỳ trên BEC":
                            return 21;
                        case "PPC nhắc định kỳ":
                        case "PPC không được chuyển thành KQKL":
                            return 11;
                        default:
                            return 11;
                    }
                //case "eTask":
                //    switch (typeName)
                //    {
                //        case "Xác nhận phát phiếu KQKLMR và đề nghị chốt số lượng KQKL MR cần đăng ký cho các thành viên tham gia":
                //        case "Xác nhận gửi biên bản sau khi họp":
                //        case "Xác nhận việc họp giao ban offline":
                //            return 21;
                //        case "Xác nhận sau cuộc họp":
                //        case "Cảnh báo đặt lịch họp nhiều ngày/tuần liên tiếp":
                //        case "Cảnh báo đặt lịch họp":
                //        case "Cảnh báo vi phạm lựa chọn thư ký cuộc họp":
                //            return 11;
                //        case "Xác nhận số lượng KQKL MR cần đăng ký sau cuộc họp":
                //            return 3;

                //        default:
                //            return 11;
                //    }
                case "HRM":
                    switch (typeName)
                    {
                        case "Công việc HRM quá hạn":
                            return 11;
                        default:
                            return 11;
                    }
                case "ACS":
                    switch (typeName)
                    {
                        case "Báo cáo kết quả công tác":
                        case "Cảnh báo đi công tác có hỗ trợ vượt mức":
                        case "Cảnh báo lịch trực ACS":
                        case "Đi công tác nhưng có dữ liệu quẹt thẻ tại văn phòng":
                        case "Cảnh báo từ hệ thống ACS":
                            return 11;
                        case "Cảnh báo cấu hình thời gian làm việc đặc biệt từ ACS":
                            return 21;
                        default:
                            return 11;
                    }
                case "Bkav Total NAC":
                    switch (typeName)
                    {
                        case "Cảnh báo vi phạm trên BTN":
                        case "Cảnh báo trên BTN":
                            return 11;
                        default:
                            return 11;
                    }
                //case "BNI":
                //    switch (typeName)
                //    {
                //        case "Cảnh báo BNI":
                //        case "Cảnh báo của hệ thống BNI":
                //        case "Backup":
                //        case "Ping Telnet":
                //        case "Sensor":
                //        case "Monitor Port":
                //        case "Folder":
                //        case "Resource":
                //        case "Windows Update":
                //        case "Website":
                //            return 11;
                //        default:
                //            return 11;
                //    }
                case "eOffice":
                    return 11;
                case "SDSS":
                    switch (typeName)
                    {
                        case "Cảnh báo từ hệ thống SDSS":
                            return 11;
                        default:
                            return 11;
                    }
                //case "KQKL":
                //    switch (typeName)
                //    {
                //        case "Xác nhận xử lý công việc KQKL thay nhân viên nghỉ đột xuất":
                //            return 21;
                //        default:
                //            return 11;
                //    }
                //case "KQKLMR":
                //    switch (typeName)
                //    {
                //        case "Báo cáo tiến độ KQKL MR":
                //        case "Công việc KQKLMR quá hạn":
                //            return 11;
                //        default:
                //            return 11;
                //    }
                case "BDS":
                    switch (typeName)
                    {
                        case "Cảnh báo từ BDS":
                            return 11;
                        default:
                            return 11;
                    }
                case "Đổi mật khẩu":
                    switch (typeName)
                    {
                        case "Cảnh báo từ hệ thống đổi mật khẩu":
                        case "Cảnh báo reset mật khẩu":
                            return 11;
                        case "Cảnh báo hết hạn mật khẩu":
                            return 21;
                        default:
                            return 11;
                    }
                case "BAM":
                    switch (typeName)
                    {
                        case "Cảnh báo từ hệ thống VirusPortal":
                        case "Cảnh báo hệ thống BAM":
                            return 11;
                        default:
                            return 11;
                    }
                case "BRA":
                    switch (typeName)
                    {
                        case "Cảnh báo từ BRA":
                        case "Cảnh báo công việc Redmine":
                        case "Cảnh báo số lượng công việc BRA vượt quá ngưỡng":
                            return 11;
                        default:
                            return 11;
                    }
                case "CBCL":
                    switch (typeName)
                    {
                        case "Xác nhận tiến độ KQKLMR đã gia hạn nhiều lần":
                        case "Áp dụng biện pháp khắc phục vi phạm CBCL":
                            return 11;
                        default:
                            return 11;
                    }
                case "Tick":
                    switch (typeName)
                    {
                        case "Vi phạm quy định":
                            return 11;
                        default:
                            return 11;
                    }
                case "BWSS":
                    switch (typeName)
                    {
                        case "Tick tập thể dục":
                            return 11;
                        default:
                            return 11;
                    }
                case "Danh bạ":
                    switch (typeName)
                    {
                        case "Cảnh báo sai thông tin cá nhân trên danh bạ":
                        case "Cảnh báo chưa cập nhật ảnh cá nhân, ảnh đại diện":
                            return 11;
                        default:
                            return 11;
                    }
                case "ePostCard":
                    switch (typeName)
                    {
                        case "Cảnh báo trên ePostCard":
                            return 21;
                        default:
                            return 11;
                    }
                case "BRMC":
                    switch (typeName)
                    {
                        case "Giao dịch BRCM chưa thanh toán":
                            return 11;
                        default:
                            return 11;
                    }
                case "BCare":
                    switch (typeName)
                    {
                        case "Cảnh báo từ Bcare":
                            return 11;
                        default:
                            return 11;
                    }
                case "BCMS":
                    switch (typeName)
                    {
                        case "Cảnh báo dữ liệu nhập trên Esoft":
                            return 11;
                        default:
                            return 11;
                    }
                case "CWS":
                    switch (typeName)
                    {
                        case "Cảnh báo BNI":
                            return 11;
                        default:
                            return 11;
                    }
                case "WEC":
                    switch (typeName)
                    {
                        case "Cảnh báo nhân viên giải trình lại nhiều lần":
                            return 11;
                        default:
                            return 11;
                    }
                //case "Từ nhiều nguồn":
                //    switch (typeName)
                //    {
                //        case "Công việc quá hạn":
                //            return 11;
                //        default:
                //            return 11;
                //    }
                //case "Chưa xác định được nguồn":
                //    switch (typeName)
                //    {
                //        case "Cảnh báo nhân viên nhiều tick":
                //        case "Báo cáo kết quả công tác":
                //        case "Cảnh báo camera bị mất kết nối":
                //        case "Cảnh báo sự cố cháy nổ":
                //        case "Cảnh báo sự cố điện":
                //        case "Data":
                //        case "Đồng bộ Voip trên danh bạ":
                //        case "Xác nhận ủy quyền Tick thể dục":
                //        case "Nghỉ phép":
                //        case "Thông báo bài viết mới vừa được đăng trên Blog":
                //        case "Xử lý số liệu Lương":
                //            return 11;
                //        case "Kiểm tra thông báo lương":
                //        case "Xác nhận vi phạm Tick lỗi phương pháp làm việc":
                //            return 21;
                //        default:
                //            return 11;
                //    }
                default:
                    return 11;
            }
        }
        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}
